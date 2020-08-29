using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mongodb.Web.Controllers
{
    public class BinaryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckValidBinaryString([FromBody] string binaryStr)
        {
            if (!string.IsNullOrEmpty(binaryStr))
            {
                var result = CheckForGoodBinaryString(binaryStr);
                return Json(result);
            }
            return Json("Please enter a valid string");
        }

        private string CheckForGoodBinaryString(string inputStr)
        {
            Regex rgx = new Regex(@"^[01]+$");
            long noOfZeros = 0;
            long noOfOnes = 0;
            List<string> prefixArr = new List<string>();
            StringBuilder sb = new StringBuilder();
            try
            {
                if (rgx.IsMatch(inputStr))
                {
                    //count no of 0's and 1's in string
                    foreach (var str in inputStr)
                    {
                        if (str.ToString() == "1")
                            noOfOnes++;
                        else if (str.ToString() == "0")
                            noOfZeros++;

                        //add new prefix to prefix array
                        sb.Append(str.ToString());
                        prefixArr.Add(sb.ToString());
                    }

                    //if no of 0's and 1's are equal than check for 
                    //For every prefix of the binary string, the number of 1's should not be less than the number of 0's.
                    if (noOfOnes == noOfZeros)
                    {
                        Console.WriteLine("Entered binary string contains equal no fo 0's and 1's.");

                        //prints all prefix for entered binary string
                        Console.WriteLine("Prefixes for given binary string are:");
                        foreach (var currentPrefix in prefixArr)
                        {
                            Console.WriteLine(currentPrefix);
                        }

                        Console.WriteLine("Now checking For every prefix of the binary string that the number of 1's should not be less than the number of 0's.");
                        foreach (var pre in prefixArr)
                        {
                            Console.WriteLine("Checking prefix: " + pre); //print msg

                            long noOfZerosInPrefix = 0;
                            long noOfOnesInPrefix = 0;
                            foreach (var p in pre)
                            {
                                if (p.ToString() == "1")
                                    noOfOnesInPrefix++;
                                else if (p.ToString() == "0")
                                    noOfZerosInPrefix++;
                            }

                            //check whether no of 1's is less than no of 0's or not
                            if (noOfOnesInPrefix < noOfZerosInPrefix)
                                return "Entered string is not a good binary string. Error occured during validation of prefix: " + pre;
                        }
                    }
                    else
                    {
                        return "No of 0's and no of 1's are not equals in entered binary string.";
                    }
                    return "Entered string is a good binary string";
                }
                else
                {
                    return "Entered string is not a valid binary string.";
                }
            }
            catch (Exception ex)
            {
                return "Error :" + ex.Message;
            }
        }
    }
}
