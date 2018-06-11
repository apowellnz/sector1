using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GalaxyCommand.Code.Game.Controllers;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class BoundsService
    {
        public static Rect GetBoundsOfObject(GameUnitController unitController)
        {
            var screenSpaceCorners = new Vector3[8];
            var theCamera = GameObject.FindObjectOfType<Camera>();

            var renderers = unitController.GetComponentInChildren<Renderer>();
            var bigBounds = renderers.bounds;
            // For each of the 8 corners of our renderer's world space bounding box,
            // convert those corners into screen space.
            screenSpaceCorners[0] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x,
                bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[1] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x,
                bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[2] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x,
                bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[3] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x,
                bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[4] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x,
                bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[5] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x,
                bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[6] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x,
                bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[7] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x,
                bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));

            // Now find the min/max X & Y of these screen space corners.
            var min_x = screenSpaceCorners[0].x;
            var min_y = screenSpaceCorners[0].y;
            var max_x = screenSpaceCorners[0].x;
            var max_y = screenSpaceCorners[0].y;

            for (var i = 1; i < 8; i++)
            {
                if (screenSpaceCorners[i].x < min_x)
                    min_x = screenSpaceCorners[i].x;
                if (screenSpaceCorners[i].y < min_y)
                    min_y = screenSpaceCorners[i].y;
                if (screenSpaceCorners[i].x > max_x)
                    max_x = screenSpaceCorners[i].x;
                if (screenSpaceCorners[i].y > max_y)
                    max_y = screenSpaceCorners[i].y;
            }
            return Rect.MinMaxRect(min_x, min_y, max_x, max_y);
        }
    }
}
