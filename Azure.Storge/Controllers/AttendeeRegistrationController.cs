﻿using Azure.Storge.Data;
using Azure.Storge.Models;
using Azure.Storge.Services;
using Microsoft.AspNetCore.Mvc;

namespace Azure.Storge.Controllers
{
    public class AttendeeRegistrationController(ITableStorageService tableStorageService,
        IBlobStorageService blobStorageService, IQueueService queueService) : Controller
    {
        public async Task<ActionResult> Index()
        {
            var data = await tableStorageService.GetAttendees();
            foreach (var item in data)
            {
                item.ImageName = await blobStorageService.GetBlobUrl(item.ImageName);
            }
            return View(data);
        }
        public async Task<ActionResult> Details(string id, string industry)
        {
            var data = await tableStorageService.GetAttendee(industry, id);
            data.ImageName = await blobStorageService.GetBlobUrl(data.ImageName);
            return View(data);
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendeeRegistrationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AttendeeEntity attendeeEntity,
            IFormFile formFile)
        {
            try
            {
                var id = Guid.NewGuid().ToString();
                attendeeEntity.PartitionKey = attendeeEntity.Industry;
                attendeeEntity.RowKey = id;

                if (formFile?.Length > 0)
                {
                    attendeeEntity.ImageName =
                        await blobStorageService.UploadBlob(formFile, id);
                }
                else
                {
                    attendeeEntity.ImageName = "default.jpg";
                }

                await tableStorageService.UpsertAttendee(attendeeEntity);

                var email = new EmailMessage
                {
                    EmailAddress = attendeeEntity.EmailAddress,
                    TimeStamp = DateTime.UtcNow,
                    Message = $"Hello {attendeeEntity.FirstName} {attendeeEntity.LastName}," +
                    $"\n\r Thank you for registering for this event. " +
                    $"\n\r Your record has been saved for future reference. "
                };
                await queueService.SendMessage(email);


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendeeRegistrationController/Edit/5
        public async Task<ActionResult> Edit(string id, string industry)
        {
            var data = await tableStorageService.GetAttendee(industry, id);

            return View(data);
        }

        // POST: AttendeeRegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AttendeeEntity attendeeEntity,
            IFormFile formFile)
        {
            try
            {
                if (formFile?.Length > 0)
                {
                    attendeeEntity.ImageName = await blobStorageService.UploadBlob(formFile, attendeeEntity.RowKey, attendeeEntity.ImageName);
                }

                attendeeEntity.PartitionKey = attendeeEntity.Industry;

                await tableStorageService.UpsertAttendee(attendeeEntity);

                var email = new EmailMessage
                {
                    EmailAddress = attendeeEntity.EmailAddress,
                    TimeStamp = DateTime.UtcNow,
                    Message = $"Hello {attendeeEntity.FirstName} {attendeeEntity.LastName}," +
                    $"\n\r Your record was modified successfully"
                };
                await queueService.SendMessage(email);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: AttendeeRegistrationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, string industry)
        {
            try
            {
                var data = await tableStorageService.GetAttendee(industry, id);
                await tableStorageService.DeleteAttendee(industry, id);
                await blobStorageService.RemoveBlob(data.ImageName);

                var email = new EmailMessage
                {
                    EmailAddress = data.EmailAddress,
                    TimeStamp = DateTime.UtcNow,
                    Message = $"Hello {data.FirstName} {data.LastName}," +
                    $"\n\r Your record was removed successfully"
                };
                await queueService.SendMessage(email);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
