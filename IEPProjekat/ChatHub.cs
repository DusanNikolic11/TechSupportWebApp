using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IEPProjekat.Models;
using Microsoft.AspNet.SignalR;

namespace IEPProjekat
{
    public class ChatHub : Hub
    {
        private AppContext db = IEPProjekat.Controllers.HomeController.getContext();
        public void Send(string name, string message, string room)
        {
            //Clients.All.broadcastMessage(name, message);
            User u = db.users.ToList().Find(x => (x.Name + " " + x.LastName) == name);
            DateTime dt = DateTime.Now;
            int Id = int.Parse(room.Split(' ')[room.Split(' ').Length - 1]);
            Channel c = db.channels.ToList().Find(x => x.Id == Id);
            Question q = new Question { Title = "ChannelMsg", Text = message, Picture = null, Category = db.categories.First(), Status = 1, Author = u, CreationTime = dt, LastLockTime = null, Replies = null, MyChannel = c };
            u.Questions.Add(q);
            db.questions.Add(q);
            db.SaveChanges();
            Clients.Group(room).broadcastMessage(name, message);
        }

        public void JoinGroup(string name, string id, string role)
        {
            String together = name + " " + id;
            int Id = int.Parse(id);
            if (role=="Agent")
            {
                Channel c = db.channels.ToList().Find(x => x.Id == Id);
                c.NumberOfAgents++;
            }
            this.Groups.Add(this.Context.ConnectionId, together);
        }

        public void LeaveGroup(string group, string role)
        {
            if (group == "")
                return;
            this.Groups.Remove(this.Context.ConnectionId, group);
        }
    }
}