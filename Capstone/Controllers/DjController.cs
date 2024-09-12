using Capstone.Models;
using Capstone.Services.Interfaces;
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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Dj dj)
    {
        var createdDj = await _djSvc.CreateDjAsync(dj);
        return RedirectToAction("Detail", new { id = createdDj.DjId });
    }

    public async Task<IActionResult> Detail(int id)
    {
        var dj = await _djSvc.GetDjByIdAsync(id);
        return View(dj);
    }

    public async Task<IActionResult> List()
    {
        var djList = await _djSvc.GetAllDjAsync();
        return View(djList);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDj(int id)
    {
        await _djSvc.DeleteDjAsync(id);
        return RedirectToAction("List");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dj = await _djSvc.GetDjByIdAsync(id);


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
            var updatedDj = await _djSvc.UpdateDjAsync(dj);
            return RedirectToAction("List");
        }

        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }


    [HttpGet]
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
            var searchEndpoint = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=artist&limit=8";
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
