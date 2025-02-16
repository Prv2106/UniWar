import grpc
from concurrent import futures
import pymysql
import statistics_pb2
import statistics_pb2_grpc
from uniwar import db_config, command_service



#TODO: 
# L'idea è quella di memorizzare le statistiche temporaneamente nella cache e solo quando una partita termmina renderle persistenti memorizzandole nel database
# Come cache utilizziamo un dizionario di dizionario di dizionario (chiave del primo dizionario nome utente, chiave del secondo dizionario numero del round  e chiave del terzo dizionario turno)
#   {"NomeUtente":{numero round: {TurnoCPU:{}}}}
# NB: La pulizia della cache avviene non appena la partita finisce e quindi devono essere memorizzati i risultati nel db


class StatisticsService(statistics_pb2_grpc.StatisticsServiceServicer):
    def SendStatistics(self, request, context):
        print(f"Ricevute statistiche per il giocatore: {request.player_id}", flush = True)
        print(f"Round: {request.round_id}, Turno utente: {request.user_turn}", flush = True)
        print(f"Territori attaccati: {request.attacked_territories}", flush = True)
        
        return statistics_pb2.Response(message="Statistiche ricevute con successo!", status = True)

    def SignIn(self, request, context):
        print(f"Ricevuta una richiesta di login con i seguenti valori -> player_id = {request.player_id}, password = {request.password}")
        pass
    
    
    
    def SignUp(self,request, context):
        print(f"Ricevuta una richiesta di registrazione con i seguenti valori -> player_id = {request.player_id}, password = {request.password}", flush = True)
        
        conn = None  
        try:
            # Apertura della connessione al database
            conn = pymysql.connect(**db_config.db_config) # Qui facciamo l'unpacking del dizionario db_config
            service = command_service.CommandService() # Creiamo un'istanza di commandService
            service.handle_register_user(command_service.RegisterUserCommand(request.player_id, request.password,conn))
            
            # Creazione della risposta di successo
            return statistics_pb2.Response(message="Utente registrato con successo!", status = True)

        except pymysql.MySQLError as err:
            # Gestione degli errori specifici del database
            if err.args[0] == 1062:  # Codice per duplicate entry (email già esistente)
                print(f"Errore di duplicazione, codice di errore: {err}", flush = True)
                return statistics_pb2.Response(message="Unavaible player_id", status = False)
            else:
                print(f"Errore durante l'inserimento nel database, codice di errore: {err}", flush = True)
                return statistics_pb2.Response(message=f"Database Error", status = False)

        except ValueError as e:
            # Gestione degli errori di validazione
            return statistics_pb2.Response(message= str(e), status = False)
        finally:
            # Chiudiamo la connessione al database
            if conn:  # Verifichiamo che conn non sia None
                conn.close()

    



def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    statistics_pb2_grpc.add_StatisticsServiceServicer_to_server(StatisticsService(), server)
    server.add_insecure_port("[::]:50051")
    server.start()
    print("Server in ascolto sulla porta 50051...", flush = True)
    server.wait_for_termination()

if __name__ == "__main__":
    serve()
