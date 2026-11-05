/// <summary>
/// Represents a tank and its color.
/// </summary>
public class Tank {
    public TankColors TankColor {get;}

    /// <summary>
    /// Initializes a tank from a numeric color index.
    /// </summary>
    /// <param name="color">Index of the <see cref="TankColors"/> value.</param>
    public Tank(int color) {
        TankColor = (TankColors) color;
    }

    /// <summary>
    /// Returns the icon file name associated with the tank color.
    /// </summary>
    /// <returns>Icon file name.</returns>
    public string GetTankIconByColor() {
        switch (TankColor) {
            case TankColors.Black:
                return "black_tank.png";
            case TankColors.Yellow:
                return "yellow_tank.png";
            case TankColors.Green:
                return "green_tank.png";
            case TankColors.Blue:
                return "blue_tank.png";
            case TankColors.Red:
                return "red_tank.png";  
            default: 
                return "black_tank.png";
            
        }
    }
    
    
}