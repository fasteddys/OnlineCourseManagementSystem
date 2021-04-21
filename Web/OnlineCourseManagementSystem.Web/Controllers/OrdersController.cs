﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCourseManagementSystem.Common;
using OnlineCourseManagementSystem.Data.Models;
using OnlineCourseManagementSystem.Services.Data;
using OnlineCourseManagementSystem.Web.ViewModels.Orders;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCourseManagementSystem.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrdersService ordersService;
        private readonly ICoursesService coursesService;
        private readonly UserManager<ApplicationUser> userManager;

        public OrdersController(IOrdersService ordersService, ICoursesService coursesService, UserManager<ApplicationUser> userManager)
        {
            this.ordersService = ordersService;
            this.coursesService = coursesService;
            this.userManager = userManager;
        }

        [Authorize(Roles = GlobalConstants.StudentRoleName)]
        [HttpPost]
        public async Task<IActionResult> Create(int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            if (this.ordersService.IsOrderAvailable(id, user.Id))
            {
                await this.ordersService.CreateAsync(id, user.Id);
                this.TempData["Message"] = "Course added successfully to cart!";
            }
            else
            {
                this.TempData["AlertMessage"] = "Order with such course and client already exists in your cart!";
            }

            return this.Redirect("/");
        }

        [Authorize(Roles = GlobalConstants.StudentRoleName)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            await this.ordersService.DeleteAsync(id, user.Id);
            this.TempData["Message"] = "Course removed successfully from cart!";
            return this.RedirectToAction(nameof(this.AllByUserId), new { user.Id });
        }

        [Authorize(Roles = GlobalConstants.StudentRoleName)]
        public async Task<IActionResult> AllByUserId()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);

            AllOrdersByUserIdListViewModel viewModel = new AllOrdersByUserIdListViewModel
            {
                Orders = this.ordersService.GetAllByUserId<AllOrdersByUserIdViewModel>(user.Id),
            };

            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Charge(string stripeEmail, string stripeToken, int id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            AllOrdersByUserIdListViewModel viewModel = new AllOrdersByUserIdListViewModel
            {
                Orders = this.ordersService.GetAllByUserId<AllOrdersByUserIdViewModel>(user.Id),
            };

            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken,
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = (int)viewModel.Orders.Sum(o => o.CoursePrice),
                Description = "Sample Charge",
                Currency = "usd",
                Customer = customer.Id,
            });

            await this.coursesService.EnrollAsync(id, user.Id);

            this.TempData["Message"] = "You enrolled successfully for the Course!";
            return this.Redirect($"/Courses/AllByUser/{user.Id}");
        }
    }
}
