namespace EmployeeMgmt.Core;

public sealed class Contractor : Employee
{
    public string Agency { get; private set; }
    public int ContractMonths { get; private set; }

    public Contractor(int id, string name, decimal hourlyRate, string agency, int contractMonths)
        : base(id, name, hourlyRate * contractMonths * 160) // rough approximation
    {
        if (string.IsNullOrWhiteSpace(agency))
            throw new ArgumentException("Agency cannot be empty", nameof(agency));
        Agency = agency;
        ContractMonths = contractMonths;
    }

    public override string Describe() => $"Contractor {Id}: {Name}, Agency: {Agency}, ContractMonths: {ContractMonths}";
}
