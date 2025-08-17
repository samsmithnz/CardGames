using CardGames.Core;
using Microsoft.AspNetCore.Mvc;

namespace CardGames.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolitaireController : ControllerBase
{
    private readonly ILogger<SolitaireController> _logger;

    public SolitaireController(ILogger<SolitaireController> logger)
    {
        _logger = logger;
    }

    [HttpPost("new-game")]
    public ActionResult<object> NewGame()
    {
        try
        {
            Deck deck = new Deck();
            deck.Shuffle();
            
            SolitaireRules game = new SolitaireRules();
            game.DealCards(deck);

            return Ok(new 
            { 
                success = true,
                tableauColumns = game.TableauColumns.Select(column => column.Select(card => new { 
                    number = (int)card.Number, 
                    suite = (int)card.Suite
                })).ToArray(),
                stockPile = game.StockPile.Select(card => new { 
                    number = (int)card.Number, 
                    suite = (int)card.Suite
                }).ToArray(),
                wastePile = game.WastePile.Select(card => new { 
                    number = (int)card.Number, 
                    suite = (int)card.Suite
                }).ToArray(),
                foundationPiles = game.FoundationPiles.Select(pile => pile.Select(card => new { 
                    number = (int)card.Number, 
                    suite = (int)card.Suite
                })).ToArray()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new game");
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    [HttpGet("test")]
    public ActionResult<object> Test()
    {
        return Ok(new { message = "CardGames.Web API is working!" });
    }
}