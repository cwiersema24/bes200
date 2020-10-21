using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryApi.Domain;
using LibraryApi.Filters;
using LibraryApi.Models;
using LibraryApi.Models.Reservations;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class ReservationsController:ControllerBase
    {

        private readonly LibraryDataContext _context;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _config;
        private readonly ILogReservations _reservationLogger;

        public ReservationsController(LibraryDataContext context, IMapper mapper, MapperConfiguration config, ILogReservations reservationLogger)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
            _reservationLogger = reservationLogger;
        }

        [HttpPost("reservations")]
        [ValidateModel]
        public async Task<ActionResult> AddReservation([FromBody] PostReservationRequest reqest)
        {
            var reservation = _mapper.Map<Reservation>(reqest);
            reservation.Status = ReservationStatus.Pending;
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            var response = _mapper.Map<ReservationDetailsResponse>(reservation);
            await _reservationLogger.WriteAsync(reservation);

            return CreatedAtRoute("reservations#getbyid", new { id = response.Id }, response);
        }
        [HttpGet("resservations/{id}", Name = "reservations#getbyid")]
        public async Task<ActionResult> GetReservationById(int id)
        {
            var reservation = await _context.Reservations
                .ProjectTo<ReservationDetailsResponse>(_config)
                .SingleOrDefaultAsync(r => r.Id == id);
            return this.Maybe(reservation);
            
        }
        [HttpPost("/reservations/accepted")]
        [ValidateModel]
        public async Task<ActionResult> ApproveReservation([FromBody] ReservationDetailsResponse reservation)
        {
            var res = await _context.Reservations.SingleOrDefaultAsync(r => r.Id == reservation.Id);
            if (res != null)
            {
                res.Status = ReservationStatus.Accepted;
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("/reservations/rejected")]
        [ValidateModel]
        public async Task<ActionResult> RejectReservation([FromBody] ReservationDetailsResponse reservation)
        {
            var res = await _context.Reservations.SingleOrDefaultAsync(r => r.Id == reservation.Id);
            if (res != null)
            {
                res.Status = ReservationStatus.Rejected;
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("/reservations/accepted")]
        public async Task<ActionResult> AcceptedReservations()
        {
            var data = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Accepted)
                .ProjectTo<ReservationDetailsResponse>(_config)
                .ToListAsync();

            return Ok(new { data, status = ReservationStatus.Accepted });
        }

        [HttpGet("/reservations/rejected")]
        public async Task<ActionResult> RejectedReservations()
        {
            var data = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Rejected)
                .ProjectTo<ReservationDetailsResponse>(_config)
                .ToListAsync();

            return Ok(new { data, status = ReservationStatus.Rejected });
        }

        [HttpGet("/reservations")]
        public async Task<ActionResult> AllReservations()
        {
            var data = await _context.Reservations
                //.Where(r => r.Status == ReservationStatus.Rejected)
                .ProjectTo<ReservationDetailsResponse>(_config)
                .ToListAsync();

            return Ok(new { data, status = "All" });
        }

        [HttpGet("/reservations/pending")]
        public async Task<ActionResult> PendingReservations()
        {
            var data = await _context.Reservations
               .Where(r => r.Status == ReservationStatus.Pending)
                .ProjectTo<ReservationDetailsResponse>(_config)
                .ToListAsync();

            return Ok(new { data, status = ReservationStatus.Pending });
        }
    }
}
