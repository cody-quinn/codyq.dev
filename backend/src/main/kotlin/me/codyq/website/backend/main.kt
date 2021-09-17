package me.codyq.website.backend

import me.codyq.website.backend.filters.RateLimitFilter
import me.codyq.website.backend.http.controllers.BlogController
import org.http4k.core.*
import org.http4k.core.Method.*
import org.http4k.core.Status.Companion.OK
import org.http4k.filter.CorsPolicy
import org.http4k.filter.ServerFilters
import org.http4k.routing.bind
import org.http4k.routing.routes
import org.http4k.server.Netty
import org.http4k.server.asServer

fun buildApp(): HttpHandler {
    val blogController = BlogController()

    val app = routes(
        "/" bind GET to blogController.getAll(),
        "/" bind POST to blogController.post(),
        "/{id}" bind GET to blogController.get(),
        "/{id}" bind PUT to blogController.update(),
        "/{id}" bind DELETE to blogController.delete()
    )

    return ServerFilters.CatchAll.invoke()
            .then(ServerFilters.RequestTracing.invoke())
            .then(ServerFilters.SetContentType.invoke(ContentType.APPLICATION_JSON))
            .then(ServerFilters.Cors.invoke(CorsPolicy.UnsafeGlobalPermissive))
            .then(RateLimitFilter.invoke())
            .then(app)
}

fun main() {
    buildApp().asServer(Netty(port = 9000)).start()
}

