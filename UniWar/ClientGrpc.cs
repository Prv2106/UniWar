using Grpc.Net.Client;
using Statistics; // Namespace generato da gRPC

namespace UniWar {
    static class ClientGrpc {
        private static readonly string _serverAddress = "http://localhost:50051";
        public static StatisticsService.StatisticsServiceClient GetStub() {
            return new StatisticsService.StatisticsServiceClient(GrpcChannel.ForAddress(_serverAddress));
        }

        public async static Task SendData(StatisticsCollection stats) {
            var stub = GetStub();

            // Mappiamo la struct C# nel messaggio gRPC
            var request = new DataRequest {
                RoundId = stats.RoundId,
                UserTurn = stats.UserTurn,
                UserOwnedTanks = stats.UserOwnedTanks,
                CpuOwnedTanks = stats.CpuOwnedTanks,
                GameId = stats.GameId,
                Username = stats.Username
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

            request.UserOwnedTerritories.AddRange(stats.UserOwnedTerritories);
            request.CpuOwnedTerritories.AddRange(stats.CpuOwnedTerritories);
            
            try {
                var response = await stub.send_dataAsync(request);
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

        
        public async static Task<GameInfoList> GetGames(string username) {
            var stub = GetStub();

            var request = new Username {
               Username_ = username
            };

            try {
                var response = await stub.get_gamesAsync(request);
                Console.WriteLine($"Risposta dal server: {response.Message}");
                return response;       
            } catch (Exception e) {
                Console.WriteLine($"Errore generico durante l'invio dei dati: {e.Message}");
                throw; // Rilancia per essere gestito dalla funzione chiamante
            }
        }


        public async static Task<StatisticsResponse> GetStatistics(int GameId) {
            var stub = GetStub();

            var request = new StatisticRequest {
                GameId = GameId
            };
            try {
                var response = await stub.get_statisticsAsync(request);
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




        public async static Task<NewGameResponse> NewGame(string Username){
            var stub = GetStub();
            var request = new Username {
                Username_ = Username
            };
            
            try {
                var response = await stub.new_gameAsync(request);
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


        public async static Task<Response> EndGame(int GameId, bool IsWin){
            var stub = GetStub();
            var request = new EndGameRequest {
                GameId = GameId,
                IsWin = IsWin
            };

            try {
                var response = await stub.end_gameAsync(request);
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

        // Gestione dell'utente
        public async static Task<Response> SignIn(string username, string password) {
            var stub = GetStub();

            var request = new SignInCredentials {
                Username = username,
                Password = password
            };

            try {
                var response = await stub.sign_inAsync(request);
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

        public async static Task<Response> SignUp(string username, string password) {
            var stub = GetStub();

            var request = new SignUpCredentials {
                Username = username,
                Password = password
            };

            try {
                var response = await stub.sign_upAsync(request);
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



        public async static Task<Response> UsernameCheck(string id) {
            var stub = GetStub();

            var request = new Username {
                Username_ = id
            };

            try {
                var response = await stub.username_checkAsync(request);
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
