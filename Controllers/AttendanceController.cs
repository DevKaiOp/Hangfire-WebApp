using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using WebApp.Common;
using WebApp.Database;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(AppDbContext context,
            ILogger<AttendanceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendances()
        {
            if (_context.Attendances == null)
            {
                return NotFound();
            }
            return await _context.Attendances.ToListAsync();
        }

        // GET: api/Attendance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Attendance>> GetAttendance(int id)
        {
            if (_context.Attendances == null)
            {
                return NotFound();
            }
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            return attendance;
        }

        // PUT: api/Attendance/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int id, Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return BadRequest();
            }

            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Attendance
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Attendance>> PostAttendance(Attendance attendance)
        {
            if (_context.Attendances == null)
            {
                return Problem("Entity set 'AppDbContext.Attendances'  is null.");
            }
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            // Enqueue background job
            var jobId = BackgroundJob.Enqueue(() => ProcessAttendance(attendance));

            // Return immediate response to the client
            return Ok(new { JobId = jobId, Status = "Processing started", id = attendance.Id });
        }

        [DisableConcurrentExecution(timeoutInSeconds: 0)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void ProcessAttendance(Attendance request)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now:dd-MMM-yyyy hh:mm:ss tt} Processing attendance Started.......");
                Thread.Sleep(5000);
                _logger.LogInformation(request.ToJson());
                _logger.LogInformation($"{DateTime.Now:dd-MMM-yyyy hh:mm:ss tt} Attendance processing completed successfully");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, $"{DateTime.Now:dd-MMM-yyyy hh:mm:ss tt} Error processing attendance");
            }
        }

        // DELETE: api/Attendance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            if (_context.Attendances == null)
            {
                return NotFound();
            }
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendanceExists(int id)
        {
            return (_context.Attendances?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
