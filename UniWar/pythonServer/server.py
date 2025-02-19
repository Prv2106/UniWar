import grpc
from concurrent import futures
import pymysql
import json
import statistics_pb2 as msg # Contiene le definizioni dei messaggi 
import statistics_pb2_grpc  # Contiene le definizioni del servizio gRPC
from uniwar import db_config, command_service, query_service, functions




class StatisticsService(statistics_pb2_grpc.StatisticsServiceServicer):
    
    # rpc per l'invio dei dati dopo una battaglia  
    def send_statistics(self, request, context):
        print(f"Ricevuti dati per il giocatore: {request.player_id}")
        
        print(f"Id partita: {request.game_id}")
        print(f"Round: {request.round_id}, Turno utente: {request.user_turn}")
        print(f"Territori difendenti: {request.defending_territories}")
        print(f"Territori attaccanti: {request.attacking_territories}")
        print(f"Territori persi: {request.lost_territories}")
        print(f"Territori posseduti dall'utente: {request.user_owned_territories}")
        print(f"Numero di carri armati posseduti dall'utente: {request.user_owned_tanks}", flush = True)
        print(f"Territori posseduti dalla CPU: {request.cpu_owned_territories}")
        print(f"Numero di carri armati posseduti dalla CPU: {request.cpu_owned_tanks}", flush = True)
        if request.user_win is  None:
            print(f"Nessuno ha vinto", flush=True)
        else: 
            print(f"Vittoria: {request.user_win}", flush=True)
        
        data = functions.Data(request)       
        
        # destrutturiamo la tupla restituita da process_data 
        (user_tanks_lost,user_territories_lost,user_perfect_defenses,cpu_tanks_lost,cpu_perfect_defenses,cpu_territories_lost,
         user_tanks_lost_attacking,cpu_tanks_lost_attacking,user_tanks_lost_defending, cpu_tanks_lost_defending)= functions.process_data(data)
        
        user_owned_territories_json = json.dumps(request.user_owned_territories)
        cpu_owned_territories_json = json.dumps(request.cpu_owned_territories)
        
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
                    turn_completed=request.turn_completed,
                    user_owned_territories=len(request.user_owned_territories),
                    user_owned_tanks=request.user_owned_tanks,
                    cpu_owned_territories=len(request.cpu_owned_territories),
                    cpu_owned_tanks=request.cpu_owned_tanks,
                    user_owned_territories_list= user_owned_territories_json, 
                    cpu_owned_territories_list= cpu_owned_territories_json, 
                    user_win=request.user_win
                    )
                )
                
                return msg.Response(message="Statistiche ricevute con successo!", status = True)

            
        
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.Response(message=f"Database Error: {err}", status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.Response(message=str(e), status=False)
    

        
    
    
    
    def get_games(self, request, context):
        print(f"Ricevuta una richiesta di GetGames per -> username = {request.player_id}", flush=True)
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                response = service.handle_get_game_query(query_service.GetGamesQuery(conn, request.username))
                
                return msg.GameInfoList(message="Storico recuperato con successo", status= True, games = response)      
        except ValueError as e:
            print(f"{e}", flush= True)
            return msg.GameInfoList(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.GameInfoList(message=f"Database Error: {err}", status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.GameInfoList(message=str(e), status=False)
        
  
    
    def get_statistics(self,request, context):
        print(f"Richiesta delle statistiche per partita con id = {request.game_id}", flush=True)
        # TODO: 
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                response = service.handle_get_game_query(query_service.GetGamesQuery(conn, request.username))
                
                return msg.GameInfoList(message="Storico recuperato con successo", status= True, games = response)      
        except ValueError as e:
            print(f"{e}", flush= True)
            return msg.GameInfoList(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.GameInfoList(message=f"Database Error: {err}", status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.GameInfoList(message=str(e), status=False)
        
        
        
        return msg.StatisticsResponse(
            user_owned_territories=21,
            cpu_owned_territories=18,
            user_owned_tanks=48,
            cpu_owned_tanks=32,
            user_owned_continents=["Europa", "Asia"],
            cpu_owned_continents=[],
            user_win= "uncompleted",
            user_tanks_lost_per_turn=3.5,
            cpu_tanks_lost_per_turn=4.6,
            user_tanks_lost_attacking=2.1,
            cpu_tanks_lost_attacking=3.0,
            user_tanks_lost_defending=1.4,
            cpu_tanks_lost_defending=1.6,
            user_perfect_defenses=5,
            cpu_perfect_defenses=3,
            user_territories_lost_per_turn=0.8,
            cpu_territories_lost_per_turn=1.2,
            user_map_ownership_percentage=55.0,
            cpu_map_ownership_percentage=45.0,
        )

        #TODO: Recuperare le statistiche dal db, definire un metodo per il calcolo dei continenti posseduti dall'utente
        
        
    # rpc da richiamare quando si vuole iniziare una nuova partita
    def new_game(self,request,context):
        pass
        
    
    # rpc da richiamare quando si termina una partita
    def end_game(self,reqquest, context):
        pass
        
        
        
    # Gestione dell'utente

    def sign_in(self, request, context):
        print(f"Ricevuta una richiesta di SignIn con i seguenti valori -> player_id = {request.player_id}, password = {request.password}", flush=True)
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                service.handle_login_user_query(query_service.LogInUserQuery(request.player_id, request.password, conn))
                print("SignIn eseguito con successo",flush=True)
                return msg.Response(message="SignIn effettuato con successo", status= True) 
            
        except ValueError as e:
            print(f"{e}", flush= True)
            return msg.Response(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.Response(message=f"Database Error: {err}", status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return msg.Response(message=str(e), status=False)
    

    def sign_up(self, request, context):
        print(f"Ricevuta una richiesta di SignUp con i seguenti valori -> player_id = {request.player_id}, password = {request.password}", flush= True)
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = command_service.CommandService()
                service.handle_register_user(command_service.RegisterUserCommand(request.player_id, request.password, conn))
                print("SignUp eseguito con successo",flush=True)

                # Creazione della risposta di successo
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
        print(f"Ricevuta richiesta per il check dello username {request.username}", flush=True)
        
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                service.handle_username_check_query(query_service.UsernameCheckQuery(conn,request.username))
                return msg.Response(message="Username valido", status = True)
            
        except ValueError as e:
            print(f"{e}", flush= True)
            return msg.Response(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return msg.Response(message=f"Database Error: {err}", status=False)
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
