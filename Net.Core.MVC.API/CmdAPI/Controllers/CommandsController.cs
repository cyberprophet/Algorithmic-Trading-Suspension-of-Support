using System.Collections.Generic;

using CmdAPI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CmdAPI.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CommandsController : ControllerBase
    {
        /*
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetString()
        {
            return new string[] { "this", "is", "hard", "coded" };
        }
        */
        [HttpDelete("{id}")]
        public ActionResult<Command> DeleteCommandItem(int id)
        {
            var commandItem = context.CommandItems.Find(id);

            if (commandItem == null)
                return NotFound();

            context.CommandItems.Remove(commandItem);
            context.SaveChanges();

            return commandItem;
        }
        [HttpPut("{id}")]
        public ActionResult PutCommandItem(int id, Command command)
        {
            if (id != command.Id)
                return BadRequest();

            context.Entry(command).State = EntityState.Modified;
            context.SaveChanges();

            return NoContent();
        }
        [HttpPost]
        public ActionResult<Command> PostCommandItem(Command command)
        {
            context.CommandItems.Add(command);
            context.SaveChanges();

            return CreatedAtAction("GetCommandItem", new Command { Id = command.Id }, command);
        }
        [HttpGet("{id}")]
        public ActionResult<Command> GetCommandItem(int id)
        {
            var commandItem = context.CommandItems.Find(id);

            if (commandItem == null)
                return NotFound();

            return commandItem;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Command>> GetCommands()
        {
            return context.CommandItems;
        }
        public CommandsController(CommandContext context) => this.context = context;
        readonly CommandContext context;
    }
}