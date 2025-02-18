using Statistics;
using System.Collections.Generic;

namespace UniWar {
    public partial class GameStatisticsView : ContentPage {
        public List<StatisticEntry> StatisticsList { get; set; } = [];
        public List<string> UserContinents {get; set;} = [];

        public GameStatisticsView(int gameId) {
            InitializeComponent();
           
            UserContinentsLabel.IsVisible = false;
            UserContinentsView.IsVisible = false;


            try {
                Console.WriteLine("Inviamo la richiesta delle statistiche");
                StatisticsResponse response = ClientGrpc.GetStatistics(gameId);
                Console.WriteLine($"Statistiche ricevute: {response}");

                StatisticsList = new List<StatisticEntry> {
                    new("Territori posseduti", response.UserOwnedTerritories, response.CpuOwnedTerritories),
                    new("Carri armati posseduti", response.UserOwnedTanks, response.CpuOwnedTanks),
                    new("Carri persi per giro", response.UserTanksLostPerTurn, response.CpuTanksLostPerTurn),
                    new("Carri persi in attacco", response.UserTanksLostAttacking, response.CpuTanksLostAttacking),
                    new("Carri persi in difesa", response.UserTanksLostDefending, response.CpuTanksLostDefending),
                    new("Difese perfette", response.UserPerfectDefenses, response.CpuPerfectDefenses),
                    new("Attacchi falliti", response.CpuPerfectDefenses, response.UserPerfectDefenses),
                    new("Territori persi per giro", response.UserTerritoriesLostPerTurn, response.CpuTerritoriesLostPerTurn),
                    new("Percentuale mappa posseduta", $"{response.UserMapOwnershipPercentage:F2}%", $"{response.CpuMapOwnershipPercentage:F2}%")
                };

               
                if(response.UserOwnedContinents != null && response.UserOwnedContinents.Any()){
                    UserContinentsLabel.IsVisible = true;
                    UserContinentsView.IsVisible = true;
                    UserContinents.Clear();
                    foreach(var continent in response.UserOwnedContinents)
                        UserContinents.Add(continent);

                }

                BindingContext = this;

            } 
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private async void OnCloseButtonClicked(object o, EventArgs args) {
            await Navigation.PopModalAsync();
        }

        public class StatisticEntry {
            public string StatName { get; }
            public string UserValue { get; }
            public string CpuValue { get; }

            public StatisticEntry(string statName, object userValue, object cpuValue) {
                StatName = statName;
                UserValue = userValue.ToString();
                CpuValue = cpuValue.ToString();
            }
        }
    }
}
