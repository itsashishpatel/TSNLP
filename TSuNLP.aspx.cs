using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

public partial class TextSummarize : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(@"Data Source=;Initial Catalog=;Integrated Security=True");
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (Session["Userid"] == "Data")
        {
                
            LinkButtonLogout.Visible = true;
            PanelMenu.Visible = false;
        }
        else
        {
            LinkButtonLogout.Visible = false;
            PanelMenu.Visible = true;
        }      

       if (Session["Char"] == "Data")
        {
            try
            {
                lblTotal.Text = Session["Cnt"].ToString();
                this.HiddenFieldchar.Value = Session["Cnt"].ToString();
                Session["Char"] = "";
            }
            catch(Exception ex)
            {

            }
        }

        if (!IsPostBack)
        {
            TextBox1.Text = "";
            TextBox2.Text = "";
            lblTotal.Text = "0";
            LabelError.Visible = false;
            LabelError1.Visible = false;
            SummarizePanel.Visible = false;
            SummarizePanel1.Visible = false;
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        {
            LabelError.Visible = true;
            LabelError1.Visible = false;
            LabelErr.Visible = false;
        }
        else if (TextBox2.Text == "")
        {
            LabelError.Visible = false;
            LabelError1.Visible = true;
            LabelErr.Visible = false;
        }

        else if (TextBox1.Text.Length <= 25)
        {
            LabelError.Visible = false;
            LabelError1.Visible = false;
            SummarizePanel.Visible = false;
            SummarizePanel1.Visible = false;
            LabelErr.Visible = true;
            LabelErr.Text = "Unable to Summarize content due to less characters or percentage value";
           
        }


        else
        {
            Session["Char"] = "Data";
            Session["Cnt"] = TextBox1.Text.Length;
            if (TextBox1.Text.Length >= 50000)
            {
                if (Session["Uid"] == "Data")
                {
                    TextSummarize();
                }
                else
                {
                    LabelError.Visible = false;
                    LabelError1.Visible = false;
                    SummarizePanel.Visible = false;
                    SummarizePanel1.Visible = false;
                    LabelErr.Visible = true;
                    LabelErr.Text = "Kindly register / login to your account";
                }
            }
            else
            {
                TextSummarize();
            }
            
		lblTotal.Text = this.HiddenFieldchar.Value;
        }
    }

    public void TextSummarize()
    {
        try
        {
            LabelError.Visible = false;
            LabelError1.Visible = false;
            SummarizePanel.Visible = true;
            SummarizePanel1.Visible = true;
            SqlCommand cmd = new SqlCommand("Delete From NewWords", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            string sTemp = TextBox1.Text;
            string[] SpliSenteces= Regex.Split(sTemp, @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])");
            string[] FinalSenteces = Regex.Split(sTemp, @"(?<=['""A-Za-z0-9][\.\!\?\'])\s+(?=[A-Z])");

           for (int i = 0; i < splitSentences.Length; i++)
            {
                splitSentences[i] = splitSentences[i].Replace(@",", "");
                splitSentences[i] = splitSentences[i].Replace(@"'", "");
                splitSentences[i] = splitSentences[i].Replace(@".", "");

                string[] words = splitSentences[i].Split(' ');
                foreach (string word in words)
                {
                    cmd = new SqlCommand("Select Word from   where Word = '" + word + "'", con);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (!(dr.HasRows))
                    {
                        con.Close();
                        cmd = new SqlCommand("Select Count from where Word Like '%" + word + "%'", con);
                        con.Open();
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();
                            int cou = Convert.ToInt32(dr[0].ToString());
                            cou += 1;
                            con.Close();
                            cmd = new SqlCommand("Update set Count ='" + cou + "' where Word Like '%" + word + "%'", con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        else
                        {
                            con.Close();
                            cmd = new SqlCommand("Insert into Values ('" + word + "','1')", con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    else
                    {
                        con.Close();
                    }
                }
            }


            if (TextBox1.Text.Length >= 5000)
            {

                int key;
                int percent = Convert.ToInt32(TextBox2.Text);
                int inputlen = FinalSenteces.Length;
                key = ((percent * inputlen) / 100) ;

                if (key > 10)

                {
                    key = key / 2;
                }
                


                SqlDataAdapter da = new SqlDataAdapter("Select Word,Count from where Count >= @key", con);
                da.SelectCommand.Parameters.AddWithValue("@key",key);
                DataSet ds = new DataSet();
                da.Fill(ds);
                int Ckey = ds.Tables[0].Rows.Count;


                cmd = new SqlCommand("Delete from ", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();





                for (int i = 0; i < Ckey; i++)
                {
                    cmd = new SqlCommand("Insert Into  Values ('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "')", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }



            }


            else if (TextBox1.Text.Length <= 1500)

            {

                SqlDataAdapter da = new SqlDataAdapter("Select Word,Count from where Count >= 5", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                int Ckey = ds.Tables[0].Rows.Count;


                cmd = new SqlCommand("Delete from ", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                for (int i = 0; i < Ckey; i++)
                {
                    cmd = new SqlCommand("Insert Into  Values ('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "')", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                }

                 da = new SqlDataAdapter("Select Word,Rank from  where Rank >= 5", con);
                 ds = new DataSet();
                 da.Fill(ds);

                int tKey = ds.Tables[0].Rows.Count;

                if (tKey <= 2)
                {
                    for (int i = 0; i < splitSentences.Length; i++)
                    {

                        string[] words = splitSentences[i].Split(' ');
                        foreach (string word in words)
                        {
                            cmd = new SqlCommand("Select Word from  where Word = '" + word + "'", con);
                            con.Open();
                            SqlDataReader dr = cmd.ExecuteReader();

                            if (!(dr.HasRows))
                            {
                                con.Close();

                                cmd = new SqlCommand("Select Word from where Word = '" + word + "'", con);
                                con.Open();
                                dr = cmd.ExecuteReader();

                                if (dr.HasRows)
                                {
                                    con.Close();
                                    cmd = new SqlCommand("Select Count from where Word Like '%" + word + "%'", con);
                                    con.Open();
                                    dr = cmd.ExecuteReader();
                                    if (dr.HasRows)
                                    {
                                        dr.Read();
                                        int cou = Convert.ToInt32(dr[0].ToString());
                                        cou += 1;
                                        con.Close();
                                        cmd = new SqlCommand("Update set Count ='" + cou + "' where Word Like '%" + word + "%'", con);
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        con.Close();
                                    }

                                    else
                                    {
                                        con.Close();
                                    }



                                }


                                else
                                {
                                    con.Close();
                                }

                            }


                            else
                            {

                                con.Close();

                            }




                        }
                    }

                }


                 da = new SqlDataAdapter("Select Word,Count from where Count >= 5", con);
                 ds = new DataSet();
                 da.Fill(ds);
                int Ckey1 = ds.Tables[0].Rows.Count;


                cmd = new SqlCommand("Delete from ", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                for (int i = 0; i < Ckey1; i++)
                {
                    cmd = new SqlCommand("Insert Into  Values ('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "')", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                }




            }




            else

            {

                SqlDataAdapter da = new SqlDataAdapter("Select Word,Count from where Count >= 5", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                int Ckey = ds.Tables[0].Rows.Count;


                cmd = new SqlCommand("Delete from ", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                for (int i = 0; i < Ckey; i++)
                {
                    cmd = new SqlCommand("Insert Into  Values ('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "')", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

            }

            cmd = new SqlCommand("Delete from Para", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            double[] SentenceWeight = new double[splitSentences.Length];
  
            for (int i = 0; i < splitSentences.Length; i++)
            {
                SentenceWeight[i] = 0;
                string[] words = splitSentences[i].Split(' ');
                foreach (string word in words)
                {
                    cmd = new SqlCommand("Select rank from  where Word Like '%" + word + "%'", con);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        int rank = Convert.ToInt32(dr[0].ToString());
                        SentenceWeight[i] += rank;
                        con.Close();
                    }
                    else
                    {
                        con.Close();
                        SentenceWeight[i] += 0.5;
                    }
     
                }
          

              for (int m = 0; m < FinalSenteces.Length; m++)
                {
                    if(FinalSenteces[i].Contains("'"))
                    {
                        FinalSenteces[i] = FinalSenteces[i].Replace("'", "");
                    }
                } 

                cmd = new SqlCommand("Insert into Values ('" + FinalSenteces[i] + "','" + SentenceWeight[i] + "','" + (i + 1) + "')", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

           
            double minValue = 0.0;
            int minIndex = 0;

            int line = Convert.ToInt32(TextBox2.Text);
            int SLine = FinalSenteces.Length;

            int final;

            if (line >= 50)
            {
                final = (line * SLine) / 100;

            }

            else

            {
                final = (line * SLine + 100 - 1) / 100;
            
            }
           

            for (int i = 0; i < final; i++)
            {
                minValue = SentenceWeight.Min();
                minIndex = SentenceWeight.ToList().IndexOf(minValue);


                var SentenceWeightlist = new List<double>(SentenceWeight);
                SentenceWeightlist.RemoveAt(minIndex);
                SentenceWeight = SentenceWeightlist.ToArray();



                var list = new List<string>(FinalSenteces);
                list.RemoveAt(minIndex);
                FinalSenteces = list.ToArray();
            }
     
           Label3.Text = "";

            for (int i = 0; i < SentenceWeight.Length; i++)
            {
                cmd = new SqlCommand("Select no,Line from where Line = '" + FinalSenteces[i] + "'", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                   

                    Label3.Text += dr[1].ToString();

                }

                con.Close();
            }
            LabelErr.Visible = false;
        }
        catch (Exception ex)
        {
            LabelError.Visible = false;
            LabelError1.Visible = false;
            SummarizePanel.Visible = false;
            SummarizePanel1.Visible = false;
            LabelErr.Visible = true;
            LabelErr.Text = "Unable to Summarize content due to some exception";
        }
    
    }

    protected void LinkButtonLogout_Click(object sender, EventArgs e)
    {

        
        Session.Abandon();       
        Response.Redirect("TextSummarize.aspx");
               
        
    }
    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {

    }
    protected void TextBox2_TextChanged(object sender, EventArgs e)
    {

    }
}