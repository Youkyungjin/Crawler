using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrawlerManagerVer2
{
    public partial class UpdateGoodsForm : Form
    {
        CGoodsData CGoodsData_ = null;

        public UpdateGoodsForm()
        {
            InitializeComponent();
        }

        public void SettingCurInfo(CGoodsData pCGoodsData)
        {
            CGoodsData_ = pCGoodsData;
            label_GoodsSeq.Text = pCGoodsData.Seq_.ToString();
            textBox_Crawler_Seq.Text  = pCGoodsData.CrawlerSeq_.ToString();
            textBox_GoodNick.Text  = pCGoodsData.GoodsNickName_;
            textBox_OptionNick.Text = pCGoodsData.OptionNickName_;

            textBox_GoodName.Text = pCGoodsData.GoodsName_;
            textBox_Option.Text = pCGoodsData.OptionName_;
        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            bool bChanged = false;

            MainForm pMainForm = (MainForm)this.Owner;
            
            Int32 CrawlerSeq = Convert.ToInt32(textBox_Crawler_Seq.Text);
            string GoodNick = textBox_GoodNick.Text;
            string optionNick = textBox_OptionNick.Text;

            if (CrawlerSeq != CGoodsData_.CrawlerSeq_)
            {
                if (CMDBInterFace.UpdateGoodsCrawlerSeq(CInfoManager.Instance.DB(), CGoodsData_.Seq_, CrawlerSeq) == true)
                {
                    CGoodsData_.CrawlerSeq_ = CrawlerSeq;
                    pMainForm.ChangeCrawlerSeq(CGoodsData_);
                    bChanged = true;
                }
            }

            if (GoodNick != CGoodsData_.GoodsNickName_ || optionNick != CGoodsData_.OptionNickName_)
            {
                if (CMDBInterFace.UpdateGoodsNickName(CInfoManager.Instance.DB(), CGoodsData_.Seq_, GoodNick, optionNick) == true)
                {
                    CGoodsData_.GoodsNickName_ = GoodNick;
                    CGoodsData_.OptionNickName_ = optionNick;
                    pMainForm.ChangeGoodsNickAndOptionNick(CGoodsData_);
                    bChanged = true;
                }
            }

            // 초기화
            CGoodsData_ = null;

            Close();

            if (bChanged == true)
                MessageBox.Show("변경 완료 되었습니다.");
        }
    }
}
