import grpc
from concurrent import futures
import pymysql
import statistics_pb2
import statistics_pb2_grpc
from uniwar import db_config, command_service, query_service




class StatisticsService(statistics_pb2_grpc.StatisticsServiceServicer):
    
    # rpc per l'invio dei dati dopo una battaglia  
    def SendStatistics(self, request, context):
        print(f"Ricevute statistiche per il giocatore: {request.player_id}")
        print(f"Round: {request.round_id}, Turno utente: {request.user_turn}")
        print(f"Territori difendenti: {request.defending_territories}")
        print(f"Territori attaccanti: {request.attacking_territories}")
        print(f"Territori persi: {request.lost_territories}")
        print(f"Territori posseduti: {request.owned_territories}")
        if request.user_win is  None:
            print(f"Nessuno ha vinto")
        print(f"Numero di carri armati totali: {request.owned_tanks}", flush = True)

        
        return statistics_pb2.Response(message="Statistiche ricevute con successo!", status = True)
    
    
    
    
    
    # Gestione dell'utente

    def SignIn(self, request, context):
        print(f"Ricevuta una richiesta di SignIn con i seguenti valori -> player_id = {request.player_id}, password = {request.password}", flush=True)
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                service.handle_login_user_query(query_service.LogInUserQuery(request.player_id, request.password, conn))
                print("SignIn eseguito con successo",flush=True)
                return statistics_pb2.Response(message="SignIn effettuato con successo", status= True) 
            
        except ValueError as e:
            print(f"{e}", flush= True)
            return statistics_pb2.Response(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return statistics_pb2.Response(message=f"Database Error: {err}", status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return statistics_pb2.Response(message=str(e), status=False)
    

    def SignUp(self, request, context):
        print(f"Ricevuta una richiesta di SignUp con i seguenti valori -> player_id = {request.player_id}, password = {request.password}", flush= True)
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = command_service.CommandService()
                service.handle_register_user(command_service.RegisterUserCommand(request.player_id, request.password, conn))
                print("SignUp eseguito con successo",flush=True)

                # Creazione della risposta di successo
                return statistics_pb2.Response(message="Utente registrato con successo!", status=True)

        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database
            if err.args[0] == 1062:  # Codice errore per duplicati (ID già esistente)
                print(f"Errore di duplicazione, codice di errore: {err}", flush=True)
                return statistics_pb2.Response(message="Unavaible player_id", status=False)
            else:
                print(f"Errore durante l'inserimento nel database, codice di errore: {err}", flush=True)
                return statistics_pb2.Response(message="Database Error", status=False)

        except ValueError as e:
            # Gestione degli errori di validazione
            return statistics_pb2.Response(message=str(e), status=False)
        
        
    def UsernameCheck(self,request,context):
        print(f"Ricevuta richiesta per il check dello username {request.username}", flush=True)
        
        try:
            with pymysql.connect(**db_config.db_config) as conn:
                service = query_service.QueryService()
                service.handle_username_check_query(query_service.UsernameCheckQuery(conn,request.username))
                return statistics_pb2.Response(message="Username valido", status = True)
            
        except ValueError as e:
            print(f"{e}", flush= True)
            return statistics_pb2.Response(message=str(e), status=False)
        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database   
            print(f"Errore nel database, codice di errore: {err}", flush=True)
            return statistics_pb2.Response(message=f"Database Error: {err}", status=False)
        except Exception as e:
            print(f"Errore generico, codice di errore: {e}", flush = True)
            return statistics_pb2.Response(message=str(e), status=False)

    



def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    statistics_pb2_grpc.add_StatisticsServiceServicer_to_server(StatisticsService(), server)
    server.add_insecure_port("[::]:50051")
    server.start()
    print("Server in ascolto sulla porta 50051...", flush = True)
    server.wait_for_termination()

if __name__ == "__main__":
    serve()
