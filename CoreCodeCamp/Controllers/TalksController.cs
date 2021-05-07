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
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : Controller
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;

        }
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetTalksByMonikerAsync(moniker, true);
                if (result == null) return NotFound();

                return _mapper.Map<TalkModel[]>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "failed to get talks");
            }
        }
        [HttpGet("{id:int}")]
        public async Task <ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var result = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (result == null) return NotFound($"Could not found the talk of {id}");

                return _mapper.Map<TalkModel>(result);

            }
            catch(Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "failed to get talks");

            }
        }
        [HttpPost]
        public  async Task<ActionResult<TalkModel>> Post(string moniker,TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return BadRequest(" camp colud'nt found");

                var talk= _mapper.Map<Talk>(model);
                talk.Camp = camp;
             
                if (model.Speaker == null) return BadRequest("Speaker is required");
                var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest(" spaeker could'nt find");
                talk.Speaker =  speaker;
                _repository.Add(talk);

                if ( await _repository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction(HttpContext, "Get", values: new { moniker, id = talk.TalkId });
                    return Created(url, _mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest(" couldnt found updated");
                }
            }
            catch(Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "failed to get talks");
            }
        }
        [HttpPut("{id:int}")]
        public async Task<object> Put(string moniker,int id, TalkModel model)
        {
            try
            {
                var talks = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talks == null) return NotFound(" talks could'not found");

                _mapper.Map(model, talks);

                if (model.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null)
                    {
                        talks.Speaker=speaker;
                    }
                }

                if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(talks);
                }
                else
                {
                    return BadRequest("Failed to update database.");
                }
            }
            catch(Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "failed to get talks");

            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<TalkModel>> Delete(string moniker,int id)
        {
            try
            {
                var talks = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talks == null) return NotFound();
                _repository.Delete(talks);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to delete talk");
                }
            }
            catch(Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "failed to get talks");
            }
        }


    }
}
