import grpc
from concurrent import futures
import pymysql 
import json
from datetime import datetime
import statistics_pb2 as msg # Contiene le definizioni dei messaggi 
import statistics_pb2_grpc  # Contiene le definizioni del servizio gRPC
import time
from zoneinfo import ZoneInfo
from uniwar import db_config, command_service, query_service, functions


class StatisticsService(statistics_pb2_grpc.StatisticsServiceServicer):
    
    # rpc per l'invio dei dati dopo una battaglia  
    def send_data(self, request, context):
        print("\n-------------------------------------------------------------------")
        print("RPC send_data:")
        print(f"Ricevuti dati per il giocatore: {request.player_id}")
        print(f"Id partita: {request.game_id}")
        print(f"Round: {request.round_id}, Turno utente: {request.user_turn}")
        print(f"Territori difendenti: {request.defending_territories}")
        print(f"Territori attaccanti: {request.attacking_territories}")
        print(f"Territori persi: {request.lost_territories}")
        print(f"Numero di Territori posseduti dall'utente: {len(request.user_owned_territories)}")
        print(f"Numero di carri armati posseduti dall'utente: {request.user_owned_tanks}")
        print(f"Numero di Territori posseduti dalla CPU: {len(request.cpu_owned_territories)}")
        print(f"Numero di carri armati posseduti dalla CPU: {request.cpu_owned_tanks}")
    
        data = functions.Data(request)
        
        # destrutturiamo la tupla restituita da process_data 
        (user_tanks_lost,user_territories_lost,user_perfect_defenses,cpu_tanks_lost,cpu_perfect_defenses,cpu_territories_lost,
         user_tanks_lost_attacking,cpu_tanks_lost_attacking,user_tanks_lost_defending, cpu_tanks_lost_defending)= functions.process_data(data)

        user_owned_territories_json = json.dumps(list(request.user_owned_territories))
        cpu_owned_territories_json = json.dumps(list(request.cpu_owned_territories))
        
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = command_service.CommandService()
                service.handle_insert_data_command(command_service.InsertDataCommand(
                    conn,
                    game_id=request.game_id,
                    round_id=request.round_id,
                    user_tanks_lost=user_tanks_lost,
                    user_tanks_lost_attacking=user_tanks_lost_attacking,
                    user_tanks_lost_defending=user_tanks_lost_defending,
                    cpu_tanks_lost=cpu_tanks_lost,
                    cpu_tanks_lost_attacking=cpu_tanks_lost_attacking,
                    cpu_tanks_lost_defending= cpu_tanks_lost_defending,
                    user_territories_lost=user_territories_lost,
                    cpu_territories_lost=cpu_territories_lost,
                    user_perfect_defenses=user_perfect_defenses,
                    cpu_perfect_defenses=cpu_perfect_defenses,
                    user_owned_territories=len(request.user_owned_territories),
                    user_owned_tanks=request.user_owned_tanks,
                    cpu_owned_territories=len(request.cpu_owned_territories),
                    cpu_owned_tanks=request.cpu_owned_tanks,
                    user_owned_territories_list= user_owned_territories_json, 
                    cpu_owned_territories_list= cpu_owned_territories_json,
                    )
                )
                print("Operazione eseguita con successo",flush=True)
                return msg.Response(message="Statistiche ricevute con successo!", status = True)        
        
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.Response(message=str(err), status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.Response(message=str(e), status=False)
    

    
    def get_games(self, request, context):
        print("\n-------------------------------------------------------------------")
        print("RPC get_games:")
        print(f"Parametri richiesta: username = {request.username}")
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                response = service.handle_get_game_query(query_service.GetGamesQuery(conn, request.username))
                game_info_list = [msg.GameInfo(id=game[0], date=game[2].strftime("%d/%m/%Y %H:%M") , state=game[3]) for game in response]
                print("Operazione eseguita con successo",flush=True)
                return msg.GameInfoList(message="Storico recuperato con successo", status= True, games = game_info_list)  
                
        except ValueError as e:
            print(f"{e}", flush= True)
            return msg.GameInfoList(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.GameInfoList(message=str(err), status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.GameInfoList(message=str(e), status=False)
        
  
    
    def get_statistics(self,request, context):
        print("\n-------------------------------------------------------------------")
        print("RPC get_statistics:")        
        print(f"Parametri richiesta:  game_id= {request.game_id}")
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                cursor,response = service.handle_get_data_query(query_service.GetDataQuery(conn, request.game_id))
                if response is not None:
                    # Estraiamo i valori dalla riga restituita dalla query
                    # description è una lista di tuple dove ogni tupla rappresenta una colonna e in cui il primo elemento è il nome della colonna  
                    columns = [desc[0] for desc in cursor.description] # lista con i nomi delle colonne
                    rows = dict(zip(columns,response)) # dizionario in modo da poter accedere ai campi utilizzando il nome
                    # Convertiamo i json in liste di stringhe
                    user_territories = json.loads(rows["user_owned_territories_list"])
                    cpu_territories = json.loads(rows["cpu_owned_territories_list"])
                    
                    # Elaboriamo le statistiche
                    
                    # Ricaviamo i continenti posseduti dall'utente e quelli posseduti dalla cpu
                    user_owned_continents = functions.get_owned_continents(user_territories)
                    cpu_owned_continents = functions.get_owned_continents(cpu_territories)
                    
                    # determiniamo il numero di giri effettivamente completati
                    completed_rounds = rows['round_id'] if rows['turn_completed'] == 2 else rows['round_id']-1
                    if completed_rounds == 0:
                        completed_rounds = 1
                                                
                    user_tanks_lost_per_round = round(rows['user_tanks_lost'] / completed_rounds, 2)
                    cpu_tanks_lost_per_round = round(rows['cpu_tanks_lost'] / completed_rounds, 2)
                    
                    user_tanks_lost_attacking_per_round = round(rows['user_tanks_lost_attacking'] / completed_rounds, 2)
                    cpu_tanks_lost_attacking_per_round = round(rows['cpu_tanks_lost_attacking'] / completed_rounds, 2)

                    user_tanks_lost_defending_per_round = round(rows['user_tanks_lost_defending'] / completed_rounds, 2)
                    cpu_tanks_lost_defending_per_round = round(rows['cpu_tanks_lost_defending'] / completed_rounds, 2)

                    user_territories_lost_per_round = round(rows['user_territories_lost'] / completed_rounds, 2)
                    cpu_territories_lost_per_round = round(rows['cpu_territories_lost'] / completed_rounds, 2)

                    
                    user_map_ownership_percentage = (rows['user_owned_territories'] / 42 ) * 100
                    cpu_map_ownership_percentage = (rows['cpu_owned_territories'] / 42 ) * 100
                    
                    print("Operazione eseguita con successo",flush=True)
                
                    return msg.StatisticsResponse(
                        message="Storico recuperato con successo", 
                        status= True,
                        user_tanks_lost_per_round = user_tanks_lost_per_round,
                        cpu_tanks_lost_per_round = cpu_tanks_lost_per_round,
                        user_tanks_lost_attacking_per_round = user_tanks_lost_attacking_per_round,
                        cpu_tanks_lost_attacking_per_round = cpu_tanks_lost_attacking_per_round,
                        user_tanks_lost_defending_per_round = user_tanks_lost_defending_per_round,
                        cpu_tanks_lost_defending_per_round = cpu_tanks_lost_defending_per_round,                    
                        user_territories_lost_per_round = user_territories_lost_per_round,                    
                        cpu_territories_lost_per_round = cpu_territories_lost_per_round,                    
                        user_map_ownership_percentage = user_map_ownership_percentage,                    
                        cpu_map_ownership_percentage = cpu_map_ownership_percentage,
                        user_owned_territories = rows['user_owned_territories'],                    
                        user_owned_tanks = rows['user_owned_tanks'],                    
                        cpu_owned_territories = rows['cpu_owned_territories'],                    
                        cpu_owned_tanks = rows['cpu_owned_tanks'],                    
                        user_owned_continents = user_owned_continents,                    
                        cpu_owned_continents = cpu_owned_continents,                    
                        user_win = rows['user_win'],                    
                        user_perfect_defenses = rows['user_perfect_defenses'],                    
                        cpu_perfect_defenses = rows['cpu_perfect_defenses'],
                        round_id = completed_rounds
                        )
                else:
                    print("Non sono disponibili dati per questa partita",flush=True)
                    return msg.StatisticsResponse(message="Non sono disponibili dati per questa partita",status= False)
        except ValueError as e:
            print(f"{e}", flush= True)
            return msg.StatisticsResponse(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.StatisticsResponse(message=str(err), status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.StatisticsResponse(message=str(e), status=False)
        
        
        
        
    # rpc da richiamare quando si vuole iniziare una nuova partita
    def new_game(self,request,context):
        print("\n-------------------------------------------------------------------")
        print("RPC new_game:")        
        print(f"Parametri richiesta: username = {request.username}")
        
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = command_service.CommandService()
                date = datetime.now(ZoneInfo("Europe/Rome")).strftime("%Y-%m-%d %H:%M")
                game_id = service.handle_insert_game_command(command_service.InsertGameCommand(conn, request.username, date))
                print("Operazione eseguita con successo",flush=True)
                return msg.NewGameResponse(game_id = game_id, status = True, message = "Operazione eseguita con successo")
    
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.NewGameResponse(message=str(err), status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.NewGameResponse(message=str(e), status=False)
        
        
        
    
    # rpc da richiamare quando si termina una partita
    def end_game(self,request, context):
        print("\n-------------------------------------------------------------------\n")
        print("RPC end_game:")  
        print(f"Parametri richiesta: game_id = {request.game_id}, is_win = {request.is_win}", flush= True)

        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = command_service.CommandService()
                service.handle_end_game_command(command_service.EndGameCommand(conn,request.game_id,request.is_win))
                return msg.Response(message="Operazione completata con successo", status=True)               
    
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.Response(message=str(err), status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.Response(message=str(e), status=False)
        
        
        
    # Gestione dell'utente

    def sign_in(self, request, context):
        print("\n-------------------------------------------------------------------\n")
        print("RPC sign_in:")  
        print(f"Parametri richiesta: username = {request.player_id}, password = {request.password}")
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                service.handle_login_user_query(query_service.LogInUserQuery(request.player_id, request.password, conn))
                print("Operazione eseguita con successo",flush=True)
                return msg.Response(message="SignIn effettuato con successo", status= True) 
            
        except ValueError as e:
            print(f"{e}", flush= True)
            return msg.Response(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.Response(message=str(err), status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.Response(message=str(e), status=False)
    

    def sign_up(self, request, context):
        print("\n-------------------------------------------------------------------\n")
        print("RPC sign_up:")  
        print(f"Parametri richiesta: username = {request.player_id}, password = {request.password}")
        
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = command_service.CommandService()
                service.handle_register_user(command_service.RegisterUserCommand(request.player_id, request.password, conn))
                print("Operazione eseguita con successo",flush=True)
                return msg.Response(message="Utente registrato con successo!", status=True)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database
            if err.args[0] == 1062:  # Codice errore per duplicati (ID già esistente)
                print(f"Errore di duplicazione, codice di errore: {err}", flush=True)
                return msg.Response(message="Unavailable player_id", status=False)
            else:
                print(f"Errore durante l'inserimento nel database, codice di errore: {err}", flush=True)
                return msg.Response(message="Database Error", status=False)

        except ValueError as e:
            # Gestione degli errori di validazione
            return msg.Response(message=str(e), status=False)
        
        
    def username_check(self,request,context):
        print("\n-------------------------------------------------------------------\n")
        print("RPC username_check:")  
        print(f"Parametri richiesta: username = {request.player_id}")
        
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                service.handle_username_check_query(query_service.UsernameCheckQuery(conn,request.username))
                print("Operazione eseguita con successo",flush=True)
                return msg.Response(message="Username valido", status = True)
            
        except ValueError as e:
            print(f"{e}", flush= True)
            return msg.Response(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.Response(message=str(err), status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.Response(message=str(e), status=False)

    



def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    statistics_pb2_grpc.add_StatisticsServiceServicer_to_server(StatisticsService(), server)
    server.add_insecure_port("[::]:50051")
    server.start()
    print("Server in ascolto sulla porta 50051...", flush = True)
    server.wait_for_termination()

if __name__ == "__main__":
    serve()
