namespace LogicPOS.Domain.Entities.POS.WorkSessions.Movements.Common;

public enum WorkSessionMovementType {
    CashDrawerOpen = 1,
    CashDrawerClose = 2,
    CashDrawerIn = 3,
    CashDrawerOut = 4,
    CashDrawerMoneyOut = 5,
    Document = 6,
    Payment = 7,
}