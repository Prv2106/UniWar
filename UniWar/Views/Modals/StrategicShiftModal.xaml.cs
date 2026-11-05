namespace UniWar
{
    /// <summary>
    /// Modal that lets the user choose how many tanks to move during a
    /// strategic shift.
    /// </summary>
    public partial class StrategicShiftModal : ContentPage
    {
        private int _selectedNumber = 1;
        private int _maxValue;
        private TaskCompletionSource<int> _tcs;


        /// <summary>
        /// Initializes the modal with the allowed range.
        /// </summary>
        /// <param name="maxValue">Maximum number of movable tanks.</param>
        /// <param name="tcs">Source completed with the chosen number.</param>
        public StrategicShiftModal(int maxValue, TaskCompletionSource<int> tcs) {
            InitializeComponent();
            _maxValue = maxValue;
            _tcs = tcs;
        }

        private void OnIncreaseClicked(object sender, EventArgs e) {
            if (_selectedNumber < _maxValue) {
                _selectedNumber++;
                numberLabel.Text = _selectedNumber.ToString();
            }
        }

        private void OnDecreaseClicked(object sender, EventArgs e) {
            if (_selectedNumber > 1) {
                _selectedNumber--;
                numberLabel.Text = _selectedNumber.ToString();
            }
        }

        private async void OnConfirmButtonClicked(object sender, EventArgs e) {
            _tcs.SetResult(_selectedNumber);
            await Navigation.PopModalAsync();
        }
    }
}
