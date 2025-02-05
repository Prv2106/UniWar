public class Tank {
    public TankColors TankColor {get;}

    public Tank(int color) {
        TankColor = (TankColors) color;
    }

    public string GetTankIconByColor() {
        switch (TankColor) {
            case TankColors.Black:
                return "black_tank.png";
            case TankColors.Purple:
                return "purple_tank.png";
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