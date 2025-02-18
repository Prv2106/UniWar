using Grpc.Net.Client;
using Statistics; // Namespace generato da gRPC

namespace UniWar {
    static class ClientGrpc {
        private static readonly string _serverAddress = "http://localhost:50051";
        public static StatisticsService.StatisticsServiceClient GetStub() {
            return new StatisticsService.StatisticsServiceClient(GrpcChannel.ForAddress(_serverAddress));
        }

        public static void SendStatisticsAsync(StatisticsCollection stats) {
            var stub = GetStub();

            // Mappiamo la struct C# nel messaggio gRPC
            var request = new Statistics.StatisticsCollection {
                RoundId = stats.RoundId,
                UserTurn = stats.UserTurn,
                OwnedTanks = stats.OwnedTanks
            };


            // Aggiungiamo AttackedTerritories alla mappa gRPC
            if (stats.DefendingTerritories != null) {
                foreach (var entry in stats.DefendingTerritories) {
                    request.DefendingTerritories.Add(entry.Key, entry.Value);
                }
            }

            if (stats.AttackingTerritories != null) {
                foreach (var entry in stats.AttackingTerritories) {
                    request.AttackingTerritories.Add(entry.Key, entry.Value);
                }
            }

            // Aggiungiamo LostTerritories alla lista gRPC
            if (stats.LostTerritories != null)
                request.LostTerritories!.AddRange(stats.LostTerritories);

            request.OwnedTerritories.AddRange(stats.OwnedTerritories);

            if (stats.UserWin != null)
                request.UserWin = (bool) stats.UserWin;

            
            try {
                var response = stub.SendStatistics(request);
                Console.WriteLine($"Risposta dal server: {response.Message}");
            }
            catch (Grpc.Core.RpcException rpcEx) {
                Console.WriteLine($"Errore gRPC: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
                throw; 
            }
            catch (Exception ex) { 
                Console.WriteLine($"Errore generico durante l'invio dei dati: {ex.Message}");
                throw; 
            }

        }




        public static Response SignIn(string username, string password){
            var stub = GetStub();


            var request = new SignInCredentials {
                PlayerId = username,
                Password = password
            };

            try {
                var response = stub.SignIn(request);
                Console.WriteLine($"Risposta dal server: {response.Message}");
                return response;
            }
            catch (Grpc.Core.RpcException rpcEx) {
                Console.WriteLine($"Errore gRPC: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
                throw; 
            }
            catch (Exception ex) { 
                Console.WriteLine($"Errore generico durante l'invio dei dati: {ex.Message}");
                throw; 
            }


        }

        public static Response SignUp(string username, string password) {
            var stub = GetStub();

            var request = new SignUpCredentials {
                PlayerId = username,
                Password = password
            };

            try {
                var response = stub.SignUp(request);
                Console.WriteLine($"Risposta dal server: {response.Message}");
                return response;       

            }
            catch (Grpc.Core.RpcException rpcEx) {
                Console.WriteLine($"Errore gRPC: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
                throw; 
            }
            catch (Exception ex) { 
                Console.WriteLine($"Errore generico durante l'invio dei dati: {ex.Message}");
                throw;
            }
        }


        public static GameInfoList GetGames(string username) {
            var stub = GetStub();

            var request = new Username() {
               Username_ = username
            };

            try {
                var response = stub.GetGames(request);
                Console.WriteLine($"Risposta dal server: {response.Message}");
                return response;       
            } catch (Exception e) {
                Console.WriteLine($"Errore generico durante l'invio dei dati: {e.Message}");
                throw; // Rilancia per essere gestito dalla funzione chiamante
            }
        }


        public static Response UsernameCheck(string id){
            var stub = GetStub();

            var request = new Username();
            request.Username_ = id;

            try{
                var response = stub.UsernameCheck(request);
                Console.WriteLine($"Risposta ricevuta dal server: {response}");
                return response;
            }
            catch (Grpc.Core.RpcException rpcEx) {
                Console.WriteLine($"Errore gRPC: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
                throw; 
            }
            catch (Exception ex) { 
                Console.WriteLine($"Errore generico durante l'invio dei dati: {ex.Message}");
                throw; 
            }


        }


        public static StatisticsResponse GetStatistics(int GameId){
            var stub = GetStub();

            var request = new StatisticRequest();
            request.GameId = GameId;
            try{
                var response = stub.GetStatistics(request);
                Console.WriteLine($"Risposta ricevuta dal server: {response}");
                return response;
            }
            catch (Grpc.Core.RpcException rpcEx) {
                Console.WriteLine($"Errore gRPC: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
                throw; 
            }
            catch (Exception ex) { 
                Console.WriteLine($"Errore generico durante l'invio dei dati: {ex.Message}");
                throw; 
            }


        }


        
    }
}
