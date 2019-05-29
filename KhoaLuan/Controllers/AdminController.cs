﻿using KhoaLuan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KhoaLuan.Helpers;
using KhoaLuan.ViewModels;
using KhoaLuan.Bo;

namespace KhoaLuan.Controllers
{
    public class AdminController : Controller
    {
		QLTroEntities db = new QLTroEntities();
		private readonly int pageSize = 8;

		public void FeedbackForUserByMail(string subject, string note, string email)
		{
			string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/Mail/templateEmail.html"));
			content = content.Replace("{{content}}", note);
			MailHelper.sendMail(email, subject, content);
		}

		#region controllers
		public ActionResult AllAccount(string pageNumber)
		{
			if (Session["account"] == null)
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				Account account = Session["account"] as Account;
				if (account.Role == 1)
				{
					return RedirectToAction("AllPost", "User");
				}
				else if (account.Role == 2)
				{
					return RedirectToAction("ChangeInfo", "Account");
				}
			}

			int pageN = 1;
			if (pageNumber != null)
			{
				pageN = Int32.Parse(pageNumber);
			}
			var listAccountManagerModel = new List<AccountManagerModel>();
			var listAllAccount = db.Accounts.Where(p=>p.Role == 1 || p.Role == 2).ToList();
			var listAccount = listAllAccount.Skip((pageN - 1) * pageSize).Take(pageSize).ToList();
			foreach (var item in listAccount)
			{
				var accountInfo = db.Infoes.SingleOrDefault(p => p.AccountID == item.AccountID);
				var accountStatus = db.AccStatus.SingleOrDefault(p => p.AccountStatusID == item.AccountStatusID);
				Province province = null;
				District district = null;
				if (accountInfo != null)
				{
					province = db.Provinces.SingleOrDefault(p => p.ProvinceID == accountInfo.ProvinceID);
					district = db.Districts.SingleOrDefault(p => p.DistrictID == accountInfo.DistrictID);
				}
				var accountManagerModelItem = new AccountManagerModel()
				{
					Account = item,
					AccountStatus = accountStatus,
					AccountInfo = accountInfo,
					Province = province,
					District = district
				};
				listAccountManagerModel.Add(accountManagerModelItem);
			}
			var subTotalPage = listAllAccount.Count / pageSize;
			var totalPage = (listAllAccount.Count % pageSize == 0) ? subTotalPage : subTotalPage + 1;
			ViewBag.PageNumber = pageN;
			ViewBag.TotalPage = totalPage;
			return View(listAccountManagerModel);
		}

		public ActionResult PostForApproval(string pageNumber)
		{
			if (Session["account"] == null)
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				Account account = Session["account"] as Account;
				if (account.Role == 1)
				{
					return RedirectToAction("AllPost", "User");
				}
				else if (account.Role == 2)
				{
					return RedirectToAction("ChangeInfo", "Account");
				}
			}

			int pageN = 1;
			if (pageNumber != null)
			{
				pageN = Int32.Parse(pageNumber);
			}

			List<PostManagerModel> list = new List<PostManagerModel>();
			var searchResult = SearchResult.CreateListSearchResult().Where(p => p.PostStatus == false).ToList();
			var postList = searchResult.Skip((pageN - 1) * pageSize).Take(pageSize).ToList();
			if (postList != null && postList.Count > 0)
			{
				foreach (var item in postList)
				{
					list.Add(new PostManagerModel()
					{
						Post = item,
						ImageList = GetImage.getListImage(item.MotelID)
					});
				}
			}
			var subTotalPage = searchResult.Count / pageSize;
			var totalPage = (searchResult.Count % pageSize == 0) ? subTotalPage : subTotalPage + 1;
			ViewBag.PageNumber = pageN;
			ViewBag.TotalPage = totalPage;
			return View(list);
		}

		[HttpGet]
		public ActionResult AllPost(string pageNumber)
		{
			if (Session["account"] == null)
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				Account account = Session["account"] as Account;
				if (account.Role == 1)
				{
					return RedirectToAction("AllPost", "User");
				}
				else if (account.Role == 2)
				{
					return RedirectToAction("ChangeInfo", "Account");
				}
			}
			int pageN = 1;
			if (pageNumber != null)
			{
				pageN = Int32.Parse(pageNumber);
			}
			List<PostManagerModel> list = new List<PostManagerModel>();

			var searchResult = SearchResult.CreateListSearchResult();
			var postList = searchResult.Skip((pageN - 1) * pageSize).Take(pageSize).ToList();
			if (postList != null && postList.Count > 0)
			{
				foreach (var item in postList)
				{
					list.Add(new PostManagerModel()
					{
						Post = item,
						ImageList = GetImage.getListImage(item.MotelID),
						motelID = item.MotelID
					});
				}
			}
			var subTotalPage = searchResult.Count / pageSize;
			var totalPage = (searchResult.Count % pageSize == 0) ? subTotalPage : subTotalPage + 1;
			ViewBag.PageNumber = pageN;
			ViewBag.TotalPage = totalPage;
			return View(list);
		}
		
		[HttpPost]
		public ActionResult AddCriteria(string criteriaName)
		{
			db.Criteria.Add(new Criterion()
			{
				CriteriaName = criteriaName
			});
			db.SaveChanges();
			return RedirectToAction("AllCriteria", "Admin");
			//return Json(new { result = true, success = true }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult AllCriteria(string pageNumber)
		{
			if (Session["account"] == null)
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				Account account = Session["account"] as Account;
				if (account.Role == 1)
				{
					return RedirectToAction("AllPost", "User");
				}
				else if (account.Role == 2)
				{
					return RedirectToAction("ChangeInfo", "Account");
				}
			}

			int pageN = 1;
			if (pageNumber != null)
			{
				pageN = Int32.Parse(pageNumber);
			}
			var listAllCriteria = db.Criteria.ToList();
			var listCriteria = listAllCriteria.Skip((pageN - 1) * pageSize).Take(pageSize).ToList();
			var subTotalPage = listAllCriteria.Count / pageSize;
			var totalPage = (listAllCriteria.Count % pageSize == 0) ? subTotalPage : subTotalPage + 1;
			ViewBag.PageNumber = pageN;
			ViewBag.TotalPage = totalPage;
			return View(listCriteria);
		}

		#endregion

		#region Method for ajax
		[HttpPost]
		public ActionResult DeleteCriteria(string criteriaID)
		{
			var criteria = db.Criteria.Single(p => p.CriteriaID.ToString() == criteriaID);
			db.Criteria.Remove(criteria);
			db.SaveChanges();
			return Json(new { result = true, success = true }, JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public ActionResult EditCriteria(string criteriaID, string criteriaName)
		{
			var criteria = db.Criteria.Single(p => p.CriteriaID.ToString() == criteriaID);
			criteria.CriteriaName = criteriaName;
			db.SaveChanges();
			return Json(new { result = true, success = true }, JsonRequestBehavior.AllowGet);
		}


		public ActionResult FeedbackForUser(string accountID,string postTitle, string message, int postID)
		{
			try
			{
				var post = db.Posts.Single(p => p.PostID == postID);
				db.Posts.Remove(post);
				db.SaveChanges();
				string email = db.Infoes.Where(p => p.AccountID.Equals(accountID)).SingleOrDefault().Email;
				FeedbackForUserByMail("Thông báo về bài viết", "Bài viết: " + postTitle + " đã bị xoá khỏi hệ thống!<br><br>" +
					"<span style='color: red'>"+message+"</span>", email);
				return Json(new { success = true, result = true }, JsonRequestBehavior.AllowGet);
			}
			catch
			{
				return Json(new { success = true, result = false }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public ActionResult LockAccount(string accountID)
		{
			var account = db.Accounts.Single(p => p.AccountID.Equals(accountID));
			account.AccountStatusID = 2;
			db.SaveChanges();
			string email = db.Infoes.Where(p => p.AccountID.Equals(accountID)).SingleOrDefault().Email;
			FeedbackForUserByMail("Thông báo về tài khoản", "Tài khoản " + accountID + " đã bị khoá bởi ban quản trị hệ thống!", email);
			return Json(new { success = true, result = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult UnBlockAccount(string accountID)
		{
			var account = db.Accounts.Single(p => p.AccountID.Equals(accountID));
			account.AccountStatusID = 1;
			db.SaveChanges();
			string email = db.Infoes.Where(p => p.AccountID.Equals(accountID)).SingleOrDefault().Email;
			FeedbackForUserByMail("Thông báo về tài khoản", "Tài khoản " + accountID + " đã sẵn sàng để bạn đăng nhập. <br> Click vào đây để đăng nhập", email);
			return Json(new { success = true, result = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult ApprovalAccount(string accountID)
		{
			var account = db.Accounts.Single(p => p.AccountID.Equals(accountID));
			account.AccountStatusID = 1;
			db.SaveChanges();
			string email = db.Infoes.Where(p => p.AccountID.Equals(accountID)).SingleOrDefault().Email;
			FeedbackForUserByMail("Thông báo về tài khoản", "Tài khoản " + accountID + " đã được duyệt bởi ban quản trị hệ thống!", email);
			return Json(new { success = true, result = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult ApprovalPost(int postId)
		{
			var current = db.Posts.FirstOrDefault(p => p.PostID.Equals(postId));
			current.PostStatus = true;
			db.SaveChanges();
			return Json(new { success = true, result = current.PostStatus }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult DissApprovalPost(int postId)
		{
			var current = db.Posts.FirstOrDefault(p => p.PostID.Equals(postId));
			current.PostStatus = false;
			db.SaveChanges();
			return Json(new { success = true, result = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GetCriteria(string motelID)
		{
			string listCriteria = string.Empty;
			string result = string.Empty;
			int ID = int.Parse(motelID);
			List<Criterion> list = db.MotelRooms.SingleOrDefault(p => p.MotelID == ID).Criteria.ToList();
			if(list.Count == 0)
			{
				return Json(new { success = false, result }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				foreach (var item in list)
				{
					listCriteria += item.CriteriaName + ", ";
				}
				int n = listCriteria.Length - 2;
				for (int i = 0; i < n; i++)
				{
					result += listCriteria[i];
				}
				return Json(new { success = true, result }, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
	}
}