namespace CovarianceAndContravariance
{
    public class Car : IVehicle
    {
        public bool IsPremiumCar { get; }

        public Car(bool isPremiumCar)
        {
            IsPremiumCar = isPremiumCar;
        }

        public decimal GetVehicleValue()
        {
            return IsPremiumCar ? 5000 : 1000;
        }
    }
}
