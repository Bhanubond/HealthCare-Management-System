using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class MASMandalService : IMASMandalService
{
    private readonly HmsDbContext _context;

    public MASMandalService(HmsDbContext context)
    {
        _context = context;
    }

    public Task<List<MASMandal>> GetAllAsync()
        => _context.MASMandals
            .Include(x => x.State)
            .OrderBy(x => x.MandalName)
            .ToListAsync();

    public Task<MASMandal?> GetByIdAsync(int id)
        => _context.MASMandals
            .Include(x => x.State)
            .FirstOrDefaultAsync(x => x.MandalId == id);

    public Task<List<MASMandal>> GetActiveMandalsByStateAsync(int stateId)
        => _context.MASMandals
            .Where(x => x.IsActive && x.StateId == stateId)
            .OrderBy(x => x.MandalName)
            .ToListAsync();

    public async Task<bool> CreateAsync(MASMandal mandal)
    {
        _context.MASMandals.Add(mandal);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(MASMandal mandal)
    {
        _context.MASMandals.Update(mandal);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var mandal = await _context.MASMandals.FindAsync(id);
        if (mandal == null) return false;

        _context.MASMandals.Remove(mandal);
        await _context.SaveChangesAsync();
        return true;
    }
}