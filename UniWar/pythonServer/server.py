import grpc
from concurrent import futures
import pymysql
import statistics_pb2 as msg # Contiene le definizioni dei messaggi 
import statistics_pb2_grpc  # Contiene le definizioni del servizio gRPC
from uniwar import db_config, command_service




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

        
        return msg.Response(message="Statistiche ricevute con successo!", status = True)
    
    
    # Gestione dell'utente

    def SignIn(self, request, context):
        print(f"Ricevuta una richiesta di login con i seguenti valori -> player_id = {request.player_id}, password = {request.password}")
        pass
    

    def SignUp(self, request, context):
        try:
            # Apertura della connessione al database con `with`
            with pymysql.connect(**db_config) as conn:
                service = command_service.CommandService()
                service.handle_register_user(command_service.RegisterUserCommand(request.player_id, request.password, conn))

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
        

    def GetGames(self, request, context):
        # "simuliamo" la query

        game_list = msg.GameInfoList(
            message="Lista partite recuperata con successo!",
            status=True
        )

        game_list.games.extend([
            msg.GameInfo(id=1, date="10/02/2024", state="vincitore"),
            msg.GameInfo(id=2, date="13/02/2024", state="perdente"),
            msg.GameInfo(id=3, date="16/02/2024", state="incompleta")
        ])

        return game_list









    



def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    statistics_pb2_grpc.add_StatisticsServiceServicer_to_server(StatisticsService(), server)
    server.add_insecure_port("[::]:50051")
    server.start()
    print("Server in ascolto sulla porta 50051...", flush = True)
    server.wait_for_termination()

if __name__ == "__main__":
    serve()
