namespace Ecommerce
{
    public class Enums
    {
        public enum UserRole
        {
            Unknown,
            Developer,
            OrderManager,
            ProductManager,
            ClientManager,
            DepartmentManager,
            SupplierManager,
            AccountingManager
        }

        public enum AccessType
        {
            Unknown,
            Edit,
            ReadOnly
        }

        public enum LegalTextType
        {
            Unknown,
            Disclaimer,
            TermsAndConditions,
            PrivacyPolicy,
            ReturnsPolicy,
            CookiePolicy
        }

        public enum NotificationType
        {
            Unknown,
            Error,
            Success,
            Info
        }

        public enum EmailTemplate
        {
            Unknown,
            AccountConfirmation,
            OrderConfirmation,
            PasswordRecovery,
            Contact
        }
    }
}
