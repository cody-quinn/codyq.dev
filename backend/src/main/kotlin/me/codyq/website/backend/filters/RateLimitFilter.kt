package me.codyq.website.backend.filters

import org.http4k.core.Filter
import org.http4k.core.Response
import org.http4k.core.Status.Companion.TOO_MANY_REQUESTS

object RateLimitFilter {

    // TODO
    operator fun invoke() = Filter { next ->
        {
            next(it)
        }
    }

}