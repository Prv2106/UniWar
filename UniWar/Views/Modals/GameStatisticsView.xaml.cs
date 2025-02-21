using Statistics;
using System.Collections.Generic;

namespace UniWar {
    public partial class GameStatisticsView : ContentPage {
        public List<StatisticEntry> StatisticsList { get; set; } = [];
        public List<string> UserContinents {get; set;} = [];
        private int GameId {get;}

        public GameStatisticsView(int gameId) {
            InitializeComponent();
           
            UserContinentsLabel.IsVisible = false;
            UserContinentsView.IsVisible = false;

            GameId = gameId;
        }

        private void ShowLoadingAnimation() {
            loading.IsVisible = true;
            page.IsVisible = false;
            warning.IsVisible = false; 
        }

        private void HideLoadingAnimation(Exception? e = null) {
            // da invocare alla fine del blocco try o del catch
            if (e is not null) {
                // ci sono problemi
                page.IsVisible = false;
                loading.IsVisible = false;
                warning.Text = e.Message;
                warning.IsVisible = true; 
            } 
            else {
                // tutto ok
                loading.IsVisible = false;
                page.IsVisible = true;
                warning.IsVisible = false; 
            }
        }
        
        private void HideLoadingAnimation(string w) {
            // da invocare alla fine del blocco try o del catch
            if(w != ""){
                page.IsVisible = false;
                loading.IsVisible = false;
                warning.Text = w;
                warning.IsVisible = true; 
            }
            else{
                // tutto ok
                loading.IsVisible = false;
                page.IsVisible = true;
                warning.IsVisible = false; 
            }
        }

        protected async override void OnAppearing() {
            base.OnAppearing();

            ShowLoadingAnimation();
            try {
                Console.WriteLine("Inviamo la richiesta delle statistiche");
                StatisticsResponse response = await ClientGrpc.GetStatistics(GameId);
                Console.WriteLine($"Statistiche ricevute: {response}");

                if (response.Status == false){
                    HideLoadingAnimation(response.Message);
                    return;
                }
                Round.Text = "(round " + response.RoundId.ToString() +"):";
                StatisticsList = new List<StatisticEntry> {
                    new("Territori posseduti", response.UserOwnedTerritories, response.CpuOwnedTerritories),
                    new("Carri armati posseduti", response.UserOwnedTanks, response.CpuOwnedTanks),
                    new("Carri armati totali persi per giro", response.UserTanksLostPerRound, response.CpuTanksLostPerRound),
                    new("Carri armati persi  in attacco per giro", response.UserTanksLostAttackingPerRound, response.CpuTanksLostAttackingPerRound),
                    new("Carri armati persi in difesa per giro", response.UserTanksLostDefendingPerRound, response.CpuTanksLostDefendingPerRound),
                    new("Difese perfette", response.UserPerfectDefenses, response.CpuPerfectDefenses),
                    new("Attacchi falliti", response.CpuPerfectDefenses, response.UserPerfectDefenses),
                    new("Territori persi per giro", response.UserTerritoriesLostPerRound, response.CpuTerritoriesLostPerRound),
                    new("Percentuale mappa posseduta", $"{response.UserMapOwnershipPercentage:F2}%", $"{response.CpuMapOwnershipPercentage:F2}%")
                };

               
                if (response.UserOwnedContinents != null && response.UserOwnedContinents.Any()) {
                    UserContinentsLabel.IsVisible = true;
                    UserContinentsView.IsVisible = true;
                    UserContinents.Clear();
                    foreach(var continent in response.UserOwnedContinents)
                        UserContinents.Add(continent);
                }

                BindingContext = this;
                HideLoadingAnimation();
            } 
            catch (Exception e) {
                Console.WriteLine(e);
                HideLoadingAnimation(e);
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
                UserValue = userValue.ToString()!;
                CpuValue = cpuValue.ToString()!;
            }
        }
    }
}
