﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Entities;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()
                 .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName))
                  .ReverseMap();

            this.CreateMap<Talk, TalkModel>()
              .ReverseMap()
              .ForMember(t => t.Camp, opt => opt.Ignore())
              .ForMember(t => t.Speaker, opt => opt.Ignore());

            this.CreateMap<Speaker, SpeakerModel>()
              .ReverseMap();
        }

    }
}