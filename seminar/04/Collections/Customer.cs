namespace Collections
{
    /// <summary>
    /// Represents Customer which has unique identifier and collection of orders.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Create globally unique identifier, more on this topic:
        /// https://msdn.microsoft.com/en-us/library/system.guid.newguid%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        public string Name { get; }

        public Customer(string name)
        {
            Name = name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return this.GetHashCode() == obj?.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
