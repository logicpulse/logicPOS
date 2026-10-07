namespace LogicPOS.Application.Features.System;

public interface IDatabaseMigrationsService
{
    public int ApplyPendingMigrations();
}