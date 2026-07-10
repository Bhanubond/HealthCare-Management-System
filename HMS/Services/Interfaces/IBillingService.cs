using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IBillingService
    {
        Task<List<BillingListItemVm>> GetUnpaidBills();
        Task<List<BillingListItemVm>> GetPaidBills();
        Task<List<PendingBillQueueVm>> GetPendingBillQueueByPatientId(int patientId);
        Task<BillingViewVm> GetBillDetailsByCaseSheetId(int caseSheetId);
        Task<int> CreateBill(int caseSheetId, int? userId = null);
        Task SavePayment(BillingPaymentVm model, int? userId = null);
        Task CancelBill(BillingCancelVm model, int? userId = null);
        Task CancelService(BillingServiceCancelVm model, int? userId = null);
    }
}
