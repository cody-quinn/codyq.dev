package me.codyq.website.backend.http.controllers

import org.http4k.core.HttpHandler
import org.http4k.core.Response
import org.http4k.core.Status.Companion.OK

class BlogController {

    fun getAll(): HttpHandler = {
        Response(OK)
    }

    fun get(): HttpHandler = {
        Response(OK)
    }

    fun post(): HttpHandler = {
        Response(OK)
    }

    fun update(): HttpHandler = {
        Response(OK)
    }

    fun delete(): HttpHandler = {
        Response(OK)
    }

}
