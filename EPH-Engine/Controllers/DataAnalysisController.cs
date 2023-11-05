using System;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Globalization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using CsvHelper.Configuration;
using EPH_Engine.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static OfficeOpenXml.ExcelErrorValue;

namespace EPH_Engine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataAnalysisController : Controller
    {
        [HttpPost("upload")]
        public IActionResult UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    var records =  csv.GetRecords<HomeBuyerInfoExceDto>().ToList();


                    var returnList = new List<HomeBuyerInfo>();
                    foreach (var item in records)
                    {


                        

                       


                        var obj = new HomeBuyerInfo()
                        {
                            ID = item.ID,
                            GrossMonthlyIncome = item.GrossMonthlyIncome,
                            CreditCardPayment = item.CreditCardPayment,
                            CarPayment = item.CarPayment,
                            StudentLoanPayments = item.StudentLoanPayments,
                            AppraisedValue = item.AppraisedValue,
                            DownPayment = item.DownPayment,
                            LoanAmount = item.LoanAmount,
                            MonthlyMortgagePayment = item.MonthlyMortgagePayment,
                            CreditScore = item.CreditScore,
                      


                        };

                        //Process the data



                        if (obj.CreditScore >= 640)
                        {
                            obj.IsEligible = true;
                        }

                        //2. LTV

                        var LTV = (obj.LoanAmount / obj.AppraisedValue) * 100;

                        if (LTV > 95)
                        {
                            obj.IsEligible = false;
                        }
                        else if (LTV > 80)
                        {
                          
                            double pmiRate = 0.01; 
                            obj.PmiCost = Math.Round(((obj.LoanAmount * pmiRate) / 12.0),2);
                            obj.IsPmiRequired = true;
                            obj.IsEligible = true;

                         
                        }
                        else
                        {
                            obj.IsEligible = false;
                        }

                        //3. DTI


                        double totalDebt = obj.CarPayment + obj.CreditCardPayment + obj.MonthlyMortgagePayment;

                        // Calculate DTI ratio
                        double dtiRatio = (totalDebt / obj.GrossMonthlyIncome) * 100.0;


                        // Check DTI thresholds
                        if (dtiRatio <= 36)
                        {
                            obj.IsEligible = true;
                        }
                        else if (dtiRatio > 36 && dtiRatio <= 43)
                        {
                            obj.IsEligible = true;
                        }
                        else
                        {
                            obj.IsEligible = false;
                        }


                        //FEDTI

                  

                        // Calculate FEDTI ratio
                        double fedtiRatio = (obj.MonthlyMortgagePayment / obj.GrossMonthlyIncome) * 100.0;

                        // Display the result
                        
                        // Check FEDTI threshold
                        if (fedtiRatio <= 28)
                        {
                            obj.IsEligible = true;
                        }
                        else
                        {
                            obj.IsEligible = false;
                        }



                        //Process the data


                        returnList.Add(obj);

                    }
                    return Ok(returnList);
                }
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error processing the CSV file: " + ex.Message);
            }
        }

        [HttpPost("upload")]
        public IActionResult getResultByAttribute()
        {

        }
        
}
