using DevExpress.MailClient.ViewModel;
using System;
using System.Collections.Generic;

namespace DevExpress.MailClient.DataProvider {
    static class TaskGenerator {
        static Random rndGenerator = new Random();

        static TaskItemViewModel GenerateTask(string subject, TaskCategory category, ContactItemViewModel contact) {
            TaskItemViewModel task = TaskItemViewModel.Create(subject, category, DateTime.Now.AddHours(-rndGenerator.Next(96)));
            int rndStatus = rndGenerator.Next(10);
            if(task.TimeDiff.TotalHours > 12) {
                task.PercentComplete = rndGenerator.Next(9) * 10;
                task.StartDate = task.CreatedDate.AddMinutes(rndGenerator.Next(720)).Date;
                if(task.TimeDiff.TotalHours > 80) {
                    task.Status = TaskStatus.Completed;
                    task.CompletedDate = task.StartDate.Value.AddHours(rndGenerator.Next(48) + 24);
                } else {
                    task.Status = TaskStatus.InProgress;
                    if(rndStatus == 6)
                        task.Status = TaskStatus.Deferred;
                    else if(rndStatus == 4 && task.PercentComplete < 40)
                        task.Status = TaskStatus.WaitingOnSomeoneElse;
                }
            }
            if(rndStatus != 5) task.DueDate = task.CreatedDate.AddHours((90 - rndStatus * 9) + 24).Date;
            if(rndStatus > 8) task.Priority = TaskPriority.High;
            if(rndStatus < 3) task.Priority = TaskPriority.Low;
            if(rndStatus != 7 && contact != null)
                task.AssignTo = contact;

            return task;
        }
        static IList<ContactItemViewModel> SelectRandomContacts(IList<ContactItemViewModel> contacts, int count) {
            if(count < 0)
                count = 10;
            if(count > contacts.Count)
                return contacts;

            var result = new List<ContactItemViewModel>();
            while(result.Count < count) {
                var contact = contacts[rndGenerator.Next(contacts.Count - 1)];
                if(!result.Contains(contact))
                    result.Add(contact);
            }
            return result;
        }
        public static IList<TaskItemViewModel> GenerateTasks(IList<ContactItemViewModel> contacts, int contactsCount = 10) {
            var contactsForTaskGenerator = SelectRandomContacts(contacts, contactsCount);
            List<TaskItemViewModel> result = new List<TaskItemViewModel>();

            foreach(var contact in contactsForTaskGenerator)
                foreach(string s in OfficeTasks)
                    result.Add(GenerateTask(s, TaskCategory.Work, contact));
            foreach(string s in HouseTasks)
                result.Add(TaskGenerator.GenerateTask(s, TaskCategory.HouseChores, null));
            foreach(string s in ShoppingTasks)
                result.Add(GenerateTask(s, TaskCategory.Shopping, null));

            return result;
        }

        static List<string> HouseTasks = new List<string>() {
            "Set-up home theater (surround sound) system",
            "Install 3 overhead lights in bedroom",
            "Change light bulbs in backyard",
            "Install a programmable thermostat",
            "Install ceiling fan in John's bedroom",
            "Install LED lights in kitchen",
            "Check wiring in main electricity panel",
            "Replace master bedroom light switch with dimmer",
            "Install an new electric outlet in garage",
            "Install electric outlet and ethernet drop in closet",
            "Install chandelier in dining room",
            "Hook up DVD Player to TV for kids",
            "Clean the House top to bottom",
            "Light cleaning of the house",
            "Clean the entire house",
            "Clean and organize basement",
            "Pick up clothes for charity event",
            "Ironing, laundry and vacuuming",
            "Take kids to park and play baseball on Sunday",
            "Clean art studio",
            "Bake brownies and send them to neighbors",
            "Assemble Kitchen Cart",
            "Move piano",
            "Clean backyard",
            "Clean out garage",
            "Organize guest bedroom",
            "Clean out closet",
            "Preapre for yard sale",
            "Sorting clothing for give-away",
            "Organize Storage Room"
        };
        static List<string> ShoppingTasks = new List<string>() {
            "Shopping at Macy's",
            "Return coffee machine",
            "Purchase plastic trash bins",
            "Shop for dinner ingredients at the store",
            "Buy new utensils for kitchen",
            "Send post card to Timothy",
            "Buy dining table and TV stand online",
            "Buy ingredients for Pasta Bolognese",
            "Size 3 diapers (3 cases)",
            "Order 3 pizzas",
            "Find out where to buy the new tablet",
            "Buy broccoli and tomatoes",
            "Buy bottle of Champagne",
            "Grocery shopping at Market Basket",
            "Find a bike at a store close to me",
            "Return jeans at JCrew",
            "Buy dog food for Fido",
            "Buy rigid foam insulation",
            "Purchase 3 24-packs of bottled Coke",
            "Purchase & deliver flowers to my home"
        };
        static List<string> OfficeTasks = new List<string>() {
            "Input bank statement transactions into Excel spreadsheet",
            "Schedule appointments and pay bills",
            "Place new address stickers on envelopes",
            "Set up and arrange appointments",
            "Copy PDF file into Word",
            "Organize business expenses (last 6 months)",
            "Return samples to vendor",
            "Organize receipts and match them up with business expenses and trips ",
            "File papers and receipts",
            "Ship out CDs to customers",
            "Respond to e-mails until noon",
            "Enter expenses into an online accounting system",
            "Conduct inventory of all furniture in office",
            "Arrange travel to conference",
            "Staple flyers to gift bags",
            "File and shred mail",
            "Print copies of brochures",
            "Enter all receipts into an Excel spreadsheet",
            "Research possible vendors",
            "Sort through paper receipts",
            "Re-package products for retail sale",
            "Scan docs, and put in desktop folder",
            "Print registration stickers"
        };
    }
}
