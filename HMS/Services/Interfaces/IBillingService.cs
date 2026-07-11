using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IBillingService
    {
        Task<List<BillingListItemVm>> GetUnpaidBills();
        Task<List<BillingListItemVm>> GetPaidBills(int? deptId = null, string? patient = null, string? billNo = null, DateTime? billDate = null);
        Task<List<PendingBillQueueVm>> GetPendingBillQueueByPatientId(int patientId);
        Task<BillingViewVm> GetPendingBillDetails(int patientId, int caseSheetId, int deptId);
        Task<BillingViewVm> GetBillDetails(int billId);
        Task<BillingViewVm> GetBillDetailsByCaseSheetId(int caseSheetId);
        Task<int> SavePayment(BillingPaymentVm model, int? userId = null);
        Task CancelBill(BillingCancelVm model, int? userId = null);
        Task CancelService(BillingServiceCancelVm model, int? userId = null);
    }
}
