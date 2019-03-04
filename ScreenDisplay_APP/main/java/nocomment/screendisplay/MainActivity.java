package nocomment.screendisplay;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Build;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.webkit.JavascriptInterface;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.EditText;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.InputStreamReader;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        final String strFilePath = this.getFilesDir().getPath() + "/index.html";
        if (!new File(strFilePath).exists()) {
            final EditText editText = new EditText(MainActivity.this);
            AlertDialog.Builder inputDialog = new AlertDialog.Builder(MainActivity.this);
            inputDialog.setTitle("请输入服务URL").setView(editText);
            inputDialog.setPositiveButton("确定",
                    new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            String strHtml = readHtmlData();
                            strHtml = strHtml.replace("0.0.0.0:0000", editText.getText().toString());
                            writeFileData(strFilePath, strHtml);
                        }
                    }).show();
        }
        WebView webView = (WebView) this.findViewById(R.id.webView);
        webView.getSettings().setJavaScriptEnabled(true);
        webView.setWebViewClient(new WebViewClient());
        webView.addJavascriptInterface(this, "app");
        webView.loadData(readFileData(strFilePath), "text/html", "utf-8");
    }

    public String readHtmlData() {
        String result = "";
        try {
            InputStream inputStream = getApplicationContext().getAssets().open("index.html");
            InputStreamReader reader = new InputStreamReader(inputStream, "utf-8");
            char[] inputBuffer = new char[inputStream.available()];
            reader.read(inputBuffer);
            result = new String(inputBuffer);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return result;
    }

    public void writeFileData(String fileName, String content) {
        try {
            FileOutputStream fos = new FileOutputStream(new File(fileName));
            byte[] bytes = content.getBytes();
            fos.write(bytes);
            fos.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public String readFileData(String fileName) {
        String result = "";
        try {
            FileInputStream fis = new FileInputStream(new File(fileName));
            int length = fis.available();
            byte[] buffer = new byte[length];
            fis.read(buffer);
            result = new String(buffer, "UTF-8");
        } catch (Exception e) {
            e.printStackTrace();
        }
        return result;
    }

    //invoke as app.fullScreen() in js
    @JavascriptInterface
    public void fullScreen() {
        //make window get focus
        new AlertDialog.Builder(this).show().dismiss();
    }

    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        if (hasFocus && Build.VERSION.SDK_INT >= 19) {
            View decorView = getWindow().getDecorView();
            decorView.setSystemUiVisibility(
                    View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                            | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                            | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                            | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                            | View.SYSTEM_UI_FLAG_FULLSCREEN
                            | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY);
        }
    }
}
