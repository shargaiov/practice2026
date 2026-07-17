namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed { get; } = 100;
        public int FirePower { get; } = 50;

        public int DistanceTraveled { get; private set; }
        public int CurrentAngle { get; private set; }
        public int RocketsFired { get; private set; }

        public void MoveForward()
        {
            DistanceTraveled += Speed;
        }

        public void Rotate(int angle)
        {
            CurrentAngle = (CurrentAngle + angle) % 360;
        }

        public void Fire()
        {
            RocketsFired++;
        }
    }
}