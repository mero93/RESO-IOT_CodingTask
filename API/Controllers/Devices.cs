using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using API.Data.DTOs;
using API.Data.Entities;
using API.Data.Repositories;
using API.Helpers;
using API.Interfaces;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:scopes")]
    public class Devices : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public Devices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<SensorDto>>> GetUserDevices()
        {
            if (User.Identity == null)
            {
                return BadRequest("Sign in required");
            }

            var clientId = User.GetClientId();

            var sensors = await _unitOfWork.Sensors.GetUserSensorsAsync(clientId);

            return Ok(sensors);
        }

        [HttpGet("statistics/{id}")]
        public async Task<ActionResult<ICollection<RecordDto>>> GetDeviceStatisticsById(int id,
            [FromQuery]int numberOfDays)
        {
            var records = await _unitOfWork.Records.ReportSelectedDevice(id, numberOfDays); 

            return Ok(records);
        }

        [HttpGet("statistics/within-distance")]
        public async Task<ActionResult<ICollection<RecordDto>>> GetDeviceStatisticsWithinDistance(
            [FromQuery]double lat, [FromQuery]double lng, [FromQuery]double distance,
            [FromQuery]int numberOfDays)
        {
            var records = await _unitOfWork.Records.ReportWithinDistance(lat, lng,
                distance, numberOfDays);

            return Ok(records);
        }

        [HttpGet("statistics/user-selection")]
        public async Task<ActionResult<ICollection<RecordDto>>> GetDeviceStatisticsUserSelection(
            [FromQuery]string sensorIdsString, [FromQuery]int numberOfDays)
        {
            var sensorIds = sensorIdsString.Split('-').Select(x => Int32.Parse(x));

            var records = await _unitOfWork.Records.ReportMultipleSelectedDevices( sensorIds,
                numberOfDays);

            return Ok();
        }

        [HttpPost("add-sensor")]
        public async Task<ActionResult<SensorDto>> AddSensor([FromBody]SensorDto properties)
        {
            var sensor = await _unitOfWork.Sensors.CreateSensorAsync(properties);

            if (sensor.Item2)
            {
                return Ok(sensor.Item1);
            }

            return BadRequest("Something went wrong. Sensor not created");
        }

        [HttpPost("{id}/edit-sensor")]
        public async Task<IActionResult> EditSensor([FromBody]SensorDto properties)
        {
            await _unitOfWork.Sensors.UpdateSensorAsync(properties);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Something went wrong. Sensor not updated");
        }

        [HttpDelete("{id}/remove-sensor")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            await _unitOfWork.Sensors.RemoveSensorAsync(id);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Something went wrong. Sensor not deleted");
        }

        [HttpPost("{id}/telemetry")]
        public async Task<ActionResult> PublishRecords(int id, 
            [FromBody]IEnumerable<RecordDto> propertyList)
        {
            foreach (var properties in propertyList)
            {
                await _unitOfWork.Records.CreateRecordAsync(id, properties);

                if ((!await _unitOfWork.Complete()))
                {
                    return BadRequest("Something went wrong. Record could not be published");
                }
            }

            return Ok();
        }

        [HttpPost("populate-sensors")]
        public async Task<ActionResult<ICollection<SensorDto>>> PopulateSensors(
            [FromQuery]double lat, [FromQuery]double lng, [FromQuery]double distance,
            [FromQuery]int numberOfSensors, [FromQuery]int recordTimer,
            [FromQuery]int totalRecords
        )
        {
            if (User.Identity == null)
            {
                return BadRequest("Sign in required");
            }

            var clientId = User.GetClientId();

            var sensors = await _unitOfWork.Sensors.PopulateSensors( lat, lng, distance, 
                numberOfSensors, clientId, recordTimer, totalRecords);

            if (!sensors.Item2)
            {
                return BadRequest("Failed to post new sensors");
            }

            return Ok(sensors.Item1);
        }

        [HttpPost("populate-records")]
        public async Task<ActionResult<ICollection<SensorDto>>> PopulateRecords([FromQuery]int numberOfSensors, 
            [FromQuery]int numberOfDays)
        {
            if (User.Identity == null)
            {
                return BadRequest("Sign in required");
            }

            var clientId = User.GetClientId();

            var sensors = await _unitOfWork.Sensors.GetLatestSensors(numberOfSensors, clientId);

            if (!(sensors.Count > 0))
            {
                return BadRequest("Failed to post new sensors");
            }

            await _unitOfWork.Records.PopulateRecords( sensors, numberOfDays);

            return Ok();
        }

        [HttpGet("get-all-sensors")]
        public async Task<ActionResult<ICollection<SensorDto>>> GetSensors()
        {
            return Ok(await _unitOfWork.Sensors.GetAllSensors());
        }

        [HttpGet("get-all-records")]
        public async Task<ActionResult<ICollection<SensorDto>>> GetRecords()
        {
            return Ok(await _unitOfWork.Records.GetAllRecords());
        }
    }
}