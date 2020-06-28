from flask import Flask, render_template, send_file, url_for, redirect
import random

app = Flask(__name__)


@app.route('/')
def home():
    return render_template('home.html')


@app.route('/api/picture')
def picture():
    return redirect(url_for('static', filename='img/bg-home/image{}.jpg'.format(random.randint(1,5))))