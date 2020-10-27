using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

[Route("v1/categories")]
public class CategoryController : ControllerBase
{
    [Route("")]
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
    {
        var categories = await context.Categories.AsNoTracking().ToListAsync();

        return Ok(categories);
    }

    [Route("{id:int}")]
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> Get(int id, [FromServices] DataContext context)
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        return category;
    }

    [Route("")]
    [HttpPost]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Post([FromBody] Category model, [FromServices] DataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            context.Categories.Add(model);
            await context.SaveChangesAsync();

            return Ok(model);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possivel criar a categoria" });
        }
    }

    [Route("{id:int}")]
    [HttpPut]
    [Authorize(Roles = "employee")]

    public async Task<ActionResult<List<Category>>> Put(int id, [FromBody] Category model, [FromServices] DataContext context)
    {
        if (model.Id != id)
            return NotFound(new { message = "Categoria não encontrada" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possivel atualizar a categoria" });
        }
    }

    [Route("{id:int}")]
    [HttpDelete]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Delete(int id, [FromServices] DataContext context)
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category == null)
            return NotFound(new { message = "Categoria não encontrada" });
        try
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(category);
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "Não foi possivel remover a categoria" });
        }
    }
}