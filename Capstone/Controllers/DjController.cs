using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class DjController : Controller
{
    private readonly IDjService _djService;

    public DjController(IDjService djService)
    {
        _djService = djService;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Dj dj)
    {
        var createdDj = await _djService.CreateDjAsync(dj);
        return RedirectToAction("Detail", new { id = createdDj.DjId });
    }

    public async Task<IActionResult> Detail(int id)
    {
        var dj = await _djService.GetDjByIdAsync(id);
        return View(dj);
    }

    public async Task<IActionResult> List()
    {
        var djList = await _djService.GetAllDjAsync();
        return View(djList);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDj(int id)
    {
        await _djService.DeleteDjAsync(id);
        return RedirectToAction("List");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dj = await _djService.GetDjByIdAsync(id);


        return View(dj);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Dj dj)
    {
        if (id != dj.DjId)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var updatedDj = await _djService.UpdateDjAsync(dj);
            return RedirectToAction("List");
        }

        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }



}
