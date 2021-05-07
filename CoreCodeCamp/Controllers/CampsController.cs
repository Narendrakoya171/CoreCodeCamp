
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Entities;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : Controller
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _imapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository campRepository, IMapper imapper,LinkGenerator linkGenarator)
        {
            _campRepository = campRepository;
            _imapper = imapper;
            _linkGenerator = linkGenarator;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includTalks = false)
        {
            try
            {
                var result = await _campRepository.GetAllCampsAsync(includTalks);
                if (result == null) return NotFound();
                return _imapper.Map<CampModel[]>(result);
                 

            }
            catch(Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failed");
            }
         
        }
        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _campRepository.GetCampAsync(moniker);
               
                if (result == null) return NotFound();

                return _imapper.Map<CampModel>(result);

            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failed");
            }

        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchBydate(DateTime date, bool includeTalks = false)
        {
            try
            {
                var result = await _campRepository.GetAllCampsByEventDate(date, includeTalks);
                if (!result.Any()) return NotFound();
                return  _imapper.Map<CampModel[]>(result);

            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failed");
            }
        }
        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var existing = await _campRepository.GetCampAsync(model.Moniker);
                if (existing != null)
                {
                    return BadRequest("Moniker in Use");
                }

                var location = _linkGenerator.GetPathByAction("Get",
                  "Camps",
                  new { moniker = model.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                // Create a new Camp
                var camp = _imapper.Map<Camp>(model);
                _campRepository.Add(camp);
                if (await _campRepository.SaveChangesAsync())
                {
                    return Created($"/api/camps/{camp.Moniker}", _imapper.Map<CampModel>(camp));
                }

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }
        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _campRepository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp with moniker of {moniker}");

                _imapper.Map(model, oldCamp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return _imapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }
        [HttpDelete("{moniker}")]
        public async Task<ActionResult<CampModel>> Delete(string moniker)
        {
            try
            {
                var oldcamp = await _campRepository.GetCampAsync(moniker);
                if (oldcamp == null) return NotFound();
                _campRepository.Delete(oldcamp);

                if( await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();

        }
    }
}
