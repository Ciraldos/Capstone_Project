using Capstone.Models;
using Capstone.Models.ViewModels;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

public class DjController : Controller
{
    private readonly IDjService _djSvc;
    private readonly ISpotifyService _spotifySvc;
    private readonly IHttpClientFactory _httpClientFactory;
    public DjController(IDjService djService, IHttpClientFactory httpClientFactory, ISpotifyService spotifyService)
    {
        _djSvc = djService;
        _spotifySvc = spotifyService;
        _httpClientFactory = httpClientFactory;
    }

    [Authorize(Policy = "AdminOrMasterPolicy")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOrMasterPolicy")]
    public async Task<IActionResult> Create(Dj dj)
    {
        var createdDj = await _djSvc.CreateDjAsync(dj);
        return RedirectToAction("Details", new { id = createdDj.DjId });
    }

    public async Task<IActionResult> Details(int id)
    {
        var dj = await _djSvc.GetDjByIdAsync(id);

        if (dj == null)
        {
            return NotFound();
        }

        var relatedEvents = await _djSvc.GetRelatedEventsAsync(id);

        var viewModel = new DjDetailsViewModel
        {
            Dj = dj,
            RelatedEvents = relatedEvents
        };

        return View(viewModel);
    }

    public async Task<IActionResult> List()
    {
        var djList = await _djSvc.GetAllDjAsync();
        return View(djList);
    }

    // GET: Dj/Delete/5
    [Authorize(Policy = "AdminOrMasterPolicy")]
    public async Task<IActionResult> Delete(int id, bool confirm = false)
    {
        if (confirm)
        {
            await _djSvc.DeleteDjAsync(id);
            return RedirectToAction("List");
        }

        var djToDelete = await _djSvc.GetDjByIdAsync(id);

        return View(djToDelete);
    }

    [Authorize(Policy = "AdminOrMasterPolicy")]

    public async Task<IActionResult> Edit(int id)
    {
        var dj = await _djSvc.GetDjByIdAsync(id);


        return View(dj);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrMasterPolicy")]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Edit(int id, Dj dj)
    {
        if (id != dj.DjId)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var updatedDj = await _djSvc.UpdateDjAsync(dj);
            return RedirectToAction("Details", new { id = dj.DjId });
        }

        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }


    [HttpGet]
    [Authorize(Policy = "AdminOrMasterPolicy")]
    public async Task<IActionResult> SearchSpotifyArtist(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return BadRequest("Query cannot be empty.");
        }

        try
        {
            // Ottieni l'access token tramite il servizio di autenticazione Spotify
            var accessToken = await _spotifySvc.GetAccessTokenAsync();

            // Crea il client HTTP utilizzando IHttpClientFactory
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Costruisci l'endpoint di ricerca con il parametro query
            var searchEndpoint = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=artist&limit=30";
            var response = await client.GetAsync(searchEndpoint);

            // Verifica se la richiesta è andata a buon fine
            if (response.IsSuccessStatusCode)
            {
                // Leggi e restituisci la risposta JSON
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            else
            {
                // Gestisci gli errori di chiamata API, ad esempio token scaduti o query malformate
                var errorMessage = $"Spotify API request failed with status code {response.StatusCode}";
                return StatusCode((int)response.StatusCode, errorMessage);
            }
        }
        catch (Exception ex)
        {
            // In caso di eccezione (ad es. problemi con l'access token o la chiamata HTTP)
            return StatusCode(500, $"An error occurred while searching for artists: {ex.Message}");
        }
    }


}
