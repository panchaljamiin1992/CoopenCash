using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using transporterQuote.Services;
using WebApplication3.Models;
using WebApplication3.ViewModels;

namespace WebApplication3.Repository
{
    public class CoopanRepository
    {
        public IEnumerable<tblTicketMaster> GetAllTickets(int userId)
        {
            using (CashCoopanEntities db = new CashCoopanEntities())
            {
                var objTickets = db.tblTicketMasters.ToList();
                var objPendingTickets = db.tblUserTickets.Where(x => x.userId == userId).ToList().Select(x => x.ticketId).ToList();

                objTickets = objTickets.Where(x => !objPendingTickets.Any(m => m.Value == x.ticketId)).ToList();

                return objTickets;
            }
        }

        public bool SubmitCoopan(AddUserCoopanModel model)
        {
            using (CashCoopanEntities db = new CashCoopanEntities())
            {
                var isExits = db.tblUserTickets.Where(x => x.ticketId == model.TicketId && x.userId == model.UserId && x.status == (int?)CoopanStatus.Completed).FirstOrDefault();
                if (isExits == null) {

                    tblUserTicket dbModel = new tblUserTicket();
                    dbModel.ticketId = model.TicketId;
                    dbModel.userId = model.UserId;
                    dbModel.ticketDate = DateTime.UtcNow;
                    dbModel.status = (int?)CoopanStatus.Completed;
                    db.tblUserTickets.Add(dbModel);
                    db.SaveChanges();

                    return true;

                }
                return false;
            }
        }
    }
}