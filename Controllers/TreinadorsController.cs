using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITATT1.Model;

namespace APITATT1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreinadorsController : ControllerBase
    {
        private readonly Contexto _context;

        public TreinadorsController(Contexto context)
        {
            _context = context;
        }

        // GET: api/Treinadors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Treinador>>> GetTreinadores()
        {
            return await _context.Treinadores.ToListAsync();
        }

        // GET: api/Treinadors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Treinador>> GetTreinador(int id)
        {
            var treinador = await _context.Treinadores.FindAsync(id);

            if (treinador == null)
            {
                return NotFound();
            }

            return treinador;
        }

        // PUT: api/Treinadors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTreinador(int id, Treinador treinador)
        {
            if (id != treinador.id)
            {
                return BadRequest();
            }

            _context.Entry(treinador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TreinadorExists(id))
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

        // POST: api/Treinadors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Treinador>> PostTreinador(Treinador treinador)
        {
            _context.Treinadores.Add(treinador);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTreinador", new { id = treinador.id }, treinador);
        }

        // DELETE: api/Treinadors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTreinador(int id)
        {
            var treinador = await _context.Treinadores.FindAsync(id);
            if (treinador == null)
            {
                return NotFound();
            }

            _context.Treinadores.Remove(treinador);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TreinadorExists(int id)
        {
            return _context.Treinadores.Any(e => e.id == id);
        }
    }
}
