using Moq;
using task04;
using Xunit;

namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            ISpaceship cruiser = new Cruiser();
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }

        [Fact]
        public void Fighter_ShouldHaveWeakerFirePowerThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.FirePower < cruiser.FirePower);
        }

        [Fact]
        public void MoveForward_ShouldIncreaseDistanceBySpeed()
        {
            var cruiser = new Cruiser();
            var fighter = new Fighter();

            cruiser.MoveForward();
            fighter.MoveForward();

            Assert.Equal(50, cruiser.DistanceTraveled);
            Assert.Equal(100, fighter.DistanceTraveled);
        }

        [Fact]
        public void Rotate_ShouldChangeAngleCorrectly()
        {
            var fighter = new Fighter();
            
            fighter.Rotate(90);
            Assert.Equal(90, fighter.CurrentAngle);

            fighter.Rotate(300);
            Assert.Equal(30, fighter.CurrentAngle); 
        }

        [Fact]
        public void Fire_ShouldIncreaseRocketsFiredCounter()
        {
            var cruiser = new Cruiser();
            cruiser.Fire();
            cruiser.Fire();
            
            Assert.Equal(2, cruiser.RocketsFired);
        }

        [Fact]
        public void ISpaceship_Mock_VerifyInterfaceMethodsAreCalled()
        {
            var mockSpaceship = new Mock<ISpaceship>();

            mockSpaceship.Object.MoveForward();
            mockSpaceship.Object.Fire();
            mockSpaceship.Object.Rotate(45);

            mockSpaceship.Verify(s => s.MoveForward(), Times.Once);
            mockSpaceship.Verify(s => s.Fire(), Times.Once);
            mockSpaceship.Verify(s => s.Rotate(45), Times.Once);
        }
    }
}