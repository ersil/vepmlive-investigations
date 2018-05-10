package com.ipm.tests;

import com.ipm.pageobjects.LoginPageObject;
import org.openqa.selenium.WebDriver;

import java.io.InputStream;
import java.util.Enumeration;
import java.util.Properties;

public class LoginStepDefinition {
//@Inject private TestProperty<WebDriver> driver;	

    protected static WebDriver driver;
    private static IpmStepDefination ipm;


    public LoginStepDefinition(WebDriver driver) {
        this.driver = driver;
    }

    public static void login(WebDriver driver) {
        LoginStepDefinition lg = new LoginStepDefinition(driver);
        driver.get(lg.getUrl());
        LoginPageObject loginPageObject = null;
        try {
            loginPageObject = new LoginPageObject(driver);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        loginPageObject.enterTheLoginCredentials(lg.getUsername(),lg.getPassword());
        loginPageObject.clickOnLoginButton();
    }

    public String getUrl() {
        return getPropValue("url");
    }
    public String getUsername() {
        return getPropValue("username");
    }
    public String getPassword() {
        return getPropValue("userpassword");
    }

    public String getPropValue(String key) {
        String filename = "local.properties";
        Properties properties = new Properties();
        try (InputStream inputStream = getClass().getClassLoader().getResourceAsStream(filename)) {
            if (inputStream == null) {
                return "";
            }
            properties.load(inputStream);

            Enumeration<?> e = properties.propertyNames();
            while (e.hasMoreElements()) {
                String propKey = (String) e.nextElement();
                if (propKey.toLowerCase().equals(key.toLowerCase())) {
                    return properties.getProperty(propKey.toLowerCase());
                }
            }
        } catch (Exception e) {
            System.out.println("Error while handling properties file : " + e);
            return "";
        }
        return "";
    }
}