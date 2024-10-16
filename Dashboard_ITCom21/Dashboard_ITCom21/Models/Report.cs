using Ganss.Excel;
namespace Dashboard_ITCom21.Models
{
    public class Report
    {
        [Column("Agent Ext")]
        public string Agent_Ext { get; set; }
        [Column("Agent Name")]
        public string Agent_Name { get; set; }
        [Column("Appointment")]
        public string Appointment { get; set; }
        [Column("Policy Appointment")]
        public string Policy_Appointment { get; set; }
        [Column("Date Of Application Date")]
        public string DateOfApplication_Date { get; set; }
        [Column("Date Of Application Month")]
        public string DateOfApplication_Month { get; set; }
        [Column("Date Of Application Year")]
        public string DateOfApplication_Year { get; set; }
        [Column("Date Of Payment Date")]
        public string DateOfPayment_Date { get; set; }
        [Column("Date Of Payment Month")]
        public string DateOfPayment_Month { get; set; }
        [Column("Date Of Payment Year")]
        public string DateOfPayment_Year { get; set; }
        [Column("Contract Number")]
        public string ContractNumber { get; set; }
        [Column("Proposer")]
        public string Proposer { get; set; }
        [Column("Proposer Gender")]
        public string ProposerGender { get; set; }
        [Column("Proposer Province")]
        public string ProposerProvince { get; set; }
        [Column("Insured Code/ID")]
        public string InsuredCodeID { get; set; }
        [Column("The Insured")]
        public string TheInsured { get; set; }
        [Column("Insured Gender")]
        public string InsuredGender { get; set; }
        [Column("Insured Province")]
        public string InsuredProvince { get; set; }
        [Column("Proposer Insured Relationship")]
        public string ProposerInsuredRelationship { get; set; }
        [Column("Age")]
        public string Age { get; set; }
        [Column("Effective Date")]
        public string Effective_Date { get; set; }
        [Column("Effective Month")]
        public string Effective_Month { get; set; }
        [Column("Effective Year")]
        public string Effective_Year { get; set; }
        [Column("Expired Date")]
        public string Expired_Date { get; set; }
        [Column("Expired Month")]
        public string Expired_Month { get; set; }
        [Column("Expired Year")]
        public string Expired_Year { get; set; }
        [Column("Plan")]
        public string Plan { get; set; }
        [Column("Patient")]
        public string Patient { get; set; }
        [Column("Dental")]
        public string Dental { get; set; }
        [Column("Accident Personal")]
        public string Accident_Personal { get; set; }
        [Column("Life Personal")]
        public string Life_Personal { get; set; }
        [Column("Pregnant")]
        public string Pregnant { get; set; }
        [Column("Premium Of Core Plan")]
        public string PremiumOfCorePlan { get; set; }
        [Column("Total Premium Of Optional Plan")]
        public string TotalPremiumOfOptionalPlan { get; set; }
        [Column("Total Premium")]
        public string TotalPremium { get; set; }
        [Column("Actually Collected After Discount")]
        public string ActuallyCollectedAfterDiscount { get; set; }
        [Column("Discount For Customer")]
        public string DiscountForCustomer { get; set; }
        [Column("Premium BV Discount")]
        public string PremiumBVDiscount { get; set; }
        [Column("Premium Asahi Discount")]
        public string PremiumAsahiDiscount { get; set; }
        [Column("Premium ITCom Discount")]
        public string PremiumITComDiscount { get; set; }
        [Column("Premium Tranfer To BaoViet")]
        public string PremiumTranferToBaoViet { get; set; }
        [Column("PDS/Manual")]
        public string PDS_Manual { get; set; }
        [Column("Number Of Call")]
        public string NumberOfCall { get; set; }
        [Column("CHANNEL")]
        public string CHANNEL { get; set; }
        [Column("First Policy Year")]
        public string FirstPolicyYear { get; set; }
        [Column("Lead Month Year")]
        public string LeadMonthYear { get; set; }
        [Column("Lead Source")]
        public string RN_LeadSource { get; set; }
        [Column("PureIntro")]
        public string RN_PureIntro { get; set; }
        [Column("Upsell Plan")]
        public string RN_UpsellPlan { get; set; }
        [Column("Upsell Rider")]
        public string RN_UpsellRider { get; set; }
        [Column("Note")]
        public string Note { get; set; }
        [Column("Payment Method")]
        public string PaymentMethod { get; set; }
        [Column("Cancelled Refund")]
        public string Cancelled_Refund { get; set; }
        [Column("Total Premium 2")]
        public string Total_Premium { get; set; }
        [Column("Date")]
        public string Total_Date { get; set; }
        [Column("Up")]
        public string Total_Up { get; set; }
        [Column("Campaign")]
        public string Campaign { get; set; }
        [Column("Payment Demand")]
        public string PaymentDemand { get; set; }
        [Column("Rounder Name")]
        public string RounderName { get; set; }
    }
}
