using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Statistics; // Namespace generato da gRPC

namespace UniWar {
    class ClientGrpc {
        public static void SendStatisticsAsync(StatisticsCollection stats) {
            
            using var channel = GrpcChannel.ForAddress("http://localhost:50051");
            var client = new StatisticsService.StatisticsServiceClient(channel);

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
                request.LostTerritories.AddRange(stats.LostTerritories);

            request.OwnedTerritories.AddRange(stats.OwnedTerritories);

            if (stats.UserWin != null)
                request.UserWin = (bool) stats.UserWin;

            
            try {
                var response = client.SendStatistics(request);
                Console.WriteLine($"Risposta dal server: {response.Message}");
            }
            catch (Grpc.Core.RpcException rpcEx) {
                Console.WriteLine($"Errore gRPC: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
                throw; // Rilancia l'eccezione per gestirla a livello superiore
            }
            catch (Exception ex) { 
                Console.WriteLine($"Errore generico durante l'invio dei dati: {ex.Message}");
                throw; // Rilancia per essere gestito dalla funzione chiamante
            }

        }




        public static Response SignIn(string username, string password){
            using var channel = GrpcChannel.ForAddress("http://localhost:50051");
            var client = new StatisticsService.StatisticsServiceClient(channel);


            var request = new SignInCredentials();
            request.PlayerId = username;
            request.Password = password;

             try {
                var response = client.SignIn(request);
                Console.WriteLine($"Risposta dal server: {response.Message}");
                return response;
            }
            catch (Grpc.Core.RpcException rpcEx) {
                Console.WriteLine($"Errore gRPC: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
                throw; // Rilancia l'eccezione per gestirla a livello superiore
            }
            catch (Exception ex) { 
                Console.WriteLine($"Errore generico durante l'invio dei dati: {ex.Message}");
                throw; // Rilancia per essere gestito dalla funzione chiamante
            }


        }

        public static Response SignUp(string username, string password){
            using var channel = GrpcChannel.ForAddress("http://localhost:50051");
            var client = new StatisticsService.StatisticsServiceClient(channel);


            var request = new SignUpCredentials();
            request.PlayerId = username;
            request.Password = password;

            try {
                var response = client.SignUp(request);
                Console.WriteLine($"Risposta dal server: {response.Message}");
                return response;       

            }
            catch (Grpc.Core.RpcException rpcEx) {
                Console.WriteLine($"Errore gRPC: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
                throw; // Rilancia l'eccezione per gestirla a livello superiore
            }
            catch (Exception ex) { 
                Console.WriteLine($"Errore generico durante l'invio dei dati: {ex.Message}");
                throw; // Rilancia per essere gestito dalla funzione chiamante
            }
            
            

        }







        
    }
}
