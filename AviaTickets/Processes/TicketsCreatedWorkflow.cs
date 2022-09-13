using AviaTickets.Models;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.Msg;
using AviaTickets.ViewModel;
using AviaTickets.ViewModel.Absractions;
using Microsoft.Extensions.Configuration;
using Scheduler;
using System;
using System.Collections.Generic;

namespace AviaTickets.Processes
{
    public class TicketsCreatedWorkflow : ITicketsCreatedWorkflow
    {        
        private ISchedulerFactory _scheduler;
        private IView _viewModel;
        private List<Data>? _data;
        private List<TicketForm> _tickets;

        private string _currency;

        public string WorkflowType { get; set; } = "TICKETS_CREATED_WORKFLOW";
        public TicketsCreatedWorkflow(IConfigurationRoot configuration
                                     , ISchedulerFactory schedulerFactory
                                     , IView viewModel)
        {           
            _viewModel = viewModel;

            _tickets = new List<TicketForm>();

            _currency = configuration["Currency"];

            _scheduler = schedulerFactory.Create()
                                         .Do(CreateTickets);
                                        
        }

        public IMessage? Start(IMessage? msg)
        {
            if (msg != default)
            {
                if (msg.IsSuccess)
                {
                    if (typeof(List<Data>) == msg.DataType)
                    {
                        _data = (List<Data>?)msg.Data;
                        return Start();
                    }
                    else throw new Exception("Input Data has incorrect type");
                    
                }
                else
                {
                    throw msg.Error ?? new Exception();
                }
            }
            else throw new Exception("Input Data is null");
        }

        public IMessage? Start()
        {
            return _scheduler.Start();            
        }              

        private IMessage? CreateTickets(IMessage? message = default)
        {
            _data?.ForEach(item => 
            {
                var ticketForm = new TicketForm();
                ticketForm.DataContext = new Tickets();
                var ticket = ticketForm.DataContext as Tickets;
                if (ticket != default)
                {
                    ticket.OpenTicketLink += _viewModel.Tickets_OpenTicketLink;
                    ticket.Link = item.Link;
                    ticket.DepCity = item.Origin;
                    ticket.ArrCity = item.Destination;
                    if (string.IsNullOrEmpty(item.ReturnAt))
                    {
                        ticket.SearchingMethod = "OneWayTicket";
                        ticket.Pic = "Resources/OneWayStrelka.jpg";
                    }
                    else
                    {
                        ticket.SearchingMethod = "ReturnTicket";
                        ticket.Pic = "Resources/ReturnWay.jpg";
                    } 
                    ticket.Time = $"{ item.Duration / 60}ч. { item.Duration % 60}мин.";
                    ticket.Transfer = $"Кол-во пересадок: {item.Transfers}";
                    ticket.ShortPrice = item.Price;
                    ticket.Price = $"{ticket.ShortPrice} {_currency}";
                    ticket.Company = $"{item.Airline}\n{item.FlightNumber}";                    

                    _tickets.Add(ticketForm);
                }
            });

            return new Message(_tickets, _tickets.GetType());
         }        
    }
}
