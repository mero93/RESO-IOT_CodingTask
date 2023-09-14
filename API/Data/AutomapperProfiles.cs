using API.Data.DTOs;
using API.Data.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Data
{
    public class AutomapperProfiles: Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<Sensor, SensorDto>()
                .ForMember(x => x.Latitude, opt => opt.MapFrom(dest => 
                    dest.Latitude * Math.Pow(10, -6)))
                .ForMember(x => x.Longitude, opt => opt.MapFrom(dest => 
                    dest.Longitude * Math.Pow(10, -6)))
                .ReverseMap()
                .ForMember(x => x.Latitude, opt => opt.MapFrom(dest => 
                    dest.Latitude * Math.Pow(10, 6)))
                .ForMember(x => x.Longitude, opt => opt.MapFrom(dest => 
                    dest.Longitude * Math.Pow(10, 6)))
                .ForMember(x => x.Records, opt => opt.Ignore());

            CreateMap<Record, RecordDto>()
                .ForMember(x => x.Time, opt => opt.MapFrom(dest => 
                    HelperMethods.UnixTimeStampToDateTime(dest.TimeStamp)))
                .ForMember(x => x.Weather, opt => opt.MapFrom(dest =>
                    HelperMethods.ConvertWeather(dest.Weather)))
                .ForMember(x => x.Latitude, opt => opt.MapFrom(dest => 
                    dest.Latitude * Math.Pow(10, -6)))
                .ForMember(x => x.Longitude, opt => opt.MapFrom(dest => 
                    dest.Longitude * Math.Pow(10, -6)));
        }
    }
}