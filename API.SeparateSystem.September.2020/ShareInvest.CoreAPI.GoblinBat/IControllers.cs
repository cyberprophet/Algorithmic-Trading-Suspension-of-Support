using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ShareInvest
{
    public interface IControllers<T>
    {
        IActionResult GetContexts();
        Task<IActionResult> GetContext(string security);
        Task<IActionResult> PostContext([FromBody] T privacy);
        Task<IActionResult> PutContext(string security, [FromBody] T privacy);
        Task<IActionResult> DeleteContext(string security);
    }
}